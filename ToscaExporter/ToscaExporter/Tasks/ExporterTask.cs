using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.Converter;
using ToscaExporter.Converter.ConverterImplementations;
using Tricentis.TCCore.Base;
using Tricentis.TCCore.BusinessObjects.Folders;
using Tricentis.TCCore.BusinessObjects.Testcases;
using Tricentis.TCCore.BusinessObjects.Testcases.Templates;
using Tricentis.TCCore.Persistency.Tasks;
using System.Reflection;
using pe = Tricentis.TCCore.Persistency;
using Tricentis.TCCore.Base.Folders;
using Newtonsoft.Json;
using MongoDB.Driver;
using ToscaExporter.ObjectModel.DataObjects;

namespace ToscaExporter.Tasks
{
    class ExporterTask : ThreadTask
    {
        private volatile object syncRoot = new object();
        private DateTime runStart;
        private string message;
        public override string Name
        {
            get
            {
                return Resources.TestExportName;
            }
        }

        protected override void RunInMainThread()
        {
            lock (syncRoot)
            {
                message = "Beginning Export";
                runStart = DateTime.Now;
            }
            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var converterTypes = thisAssembly.GetTypes().Where(x => x.CustomAttributes.Any(attr => attr.AttributeType == typeof(ConvertedTypeAttribute))
                                                            && typeof(IConverter).IsAssignableFrom(x)).ToList();
            List<IConverter> converters = new List<IConverter>();
            foreach (var converterType in converterTypes)
            {
                converters.Add((IConverter)Activator.CreateInstance(converterType));
            }

            var folder = this.Object as Tricentis.TCCore.Base.Folders.OwnedFolder;
            //if (!folder.PossibleContent.ToLower().Contains("testcase"))
            //    throw new InvalidOperationException("This task is intended for folders containing test cases");

            var convertedObjects = GetReportableObjectsForFolder(folder, converters);

            MongoDB.Driver.MongoClient client = new MongoDB.Driver.MongoClient();
            var db = client.GetDatabase("tosca");
            var snapshots = db.GetCollection<Snapshot>("snapshots");
            var reportableObjects = db.GetCollection<ReportableObject>("reportable_objects");
            var errorLogs = db.GetCollection<ErrorLog>("errors");
            var objectHistories = db.GetCollection<ObjectHistory>("object_history");
            var currentSnapshot = new Snapshot { ID = Guid.NewGuid(), SnapshotDate = DateTime.Now };
            List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
            Queue<ReportableObject> objectsToLink = new Queue<ReportableObject>();
            snapshots.InsertOne(currentSnapshot);

            foreach (var objList in convertedObjects)
            {
                string baseMessage = message;
                for (int i = 0; i < objList.Length; i++)
                {
                    var obj = objList[i];
                    lock (syncRoot)
                    {
                        message = $"{baseMessage} - {(((double)i / (double)objList.Length) * 100).ToString("##")}%";
                    }
                    try
                    {
                        var task = InsertObject(obj, reportableObjects, objectHistories, currentSnapshot);
                        tasks.Add(task);

                        objectsToLink.Enqueue(obj);
                        Task.WaitAll(tasks.ToArray());
                    }
                    catch (Exception e)
                    {
                        errorLogs.InsertOneAsync(new ErrorLog { Error = $"An error occured, overwriting object {obj.ID} \r\n{e.Message}", Stacktrace = e.StackTrace, DateTime = DateTime.Now });
                    }
                    lock (syncRoot)
                    {
                        //message = $"Saved object {i + 1} of {result.Length}";
                    }
                    tasks.Clear();
                }
            }
            
            while (objectsToLink.Count > 0)
            {
                lock(syncRoot)
                {
                    message = $"Constructing analysis - {objectsToLink.Count} remaining";
                }
                var objToLink = objectsToLink.Dequeue();
                tasks.Add(LinkObject(objToLink, reportableObjects));
                var metaDataStoreTask = StoreMetadataAsync(objToLink, db.GetCollection<EntityDefinition>("entity_definitions"), reportableObjects);
                tasks.Add(metaDataStoreTask);
                if (tasks.Count > 50)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                }
            }
        }

        private async Task<ReportableObject> InsertObject(ReportableObject obj, IMongoCollection<ReportableObject> reportableObjects, IMongoCollection<ObjectHistory> objectHistories, Snapshot currentSnapshot)
        {
            var existingObjectCursor = await reportableObjects.FindAsync(x => x.ID == obj.ID);
            var existingObjects = existingObjectCursor.ToList();
            if (existingObjects.Count == 0)
            {
                objectHistories.InsertOne(new ObjectHistory { ReportableObject = obj, Snapshot = currentSnapshot });
                reportableObjects.InsertOne(obj);
                return obj;
            }
            else
            {
                var storedObj = existingObjects.First();
                try
                {
                    if (storedObj.Hash == null && obj.Hash == null)
                        return obj;
                    if (storedObj.Hash != null && obj.Hash != null)
                        if (storedObj.Hash.SequenceEqual(obj.Hash))
                            return obj; //no change
                    await objectHistories.InsertOneAsync(new ObjectHistory { ReportableObject = storedObj, Snapshot = currentSnapshot });
                    await reportableObjects.ReplaceOneAsync(x => x.ID == obj.ID, obj);
                    return obj;
                }
                catch (Exception e)
                {
                    throw e;
                } //If an error occurs, 
                  ///TODO:
                  ///Pass this query task off to the DB to do the comparison for latency
            }
        }

        private async Task LinkObject(ReportableObject objectToLink, IMongoCollection<ReportableObject> objects)
        {
            foreach (var link in objectToLink.Links)
            {
                if (link == null || link.LinkedObjects == null || link.LinkedObjects.Length == 0)
                    continue;
                foreach (var linkedObject in link.LinkedObjects)
                {
                    var matchingObjectCursor = await objects.FindAsync<ReportableObject>(r => r.ID == linkedObject);
                    var matchedObjects = matchingObjectCursor.ToList();
                    if (matchedObjects.Count == 0)
                        continue;
                    var match = matchedObjects.First();
                    if (match.ReferencedFrom == null)
                        match.ReferencedFrom = new List<ReferencingObject>();
                    match.ReferencedFrom.RemoveAll(x => x.ReferringObject.ID == objectToLink.ID);
                    match.ReferencedFrom.Add(new ReferencingObject { LinkDescription = link.LinkDescription, ReferringObject = objectToLink });
                    await objects.FindOneAndReplaceAsync<ReportableObject>(r => r.ID == match.ID, match);
                }
            }
        }

        protected override void RunInObserverThread()
        {
            TaskContext.ShowStatusInfo($"{message} - {DateTime.Now.Subtract(runStart).Minutes}:{DateTime.Now.Subtract(runStart).Seconds}");
        }

        protected override bool IsAbortable
        {
            get
            {
                return true;
            }
        }

        private async Task StoreMetadataAsync(ReportableObject obj, IMongoCollection<EntityDefinition> entityDefinitions, IMongoCollection<ReportableObject> objects)
        {
            List<Task> tasks = new List<Task>();
            var entityDefinitionMatches = await entityDefinitions.FindAsync(x => x.EntityType == obj.Type);

            var ed = entityDefinitionMatches.FirstOrDefault();
            if (ed == null)
                ed = new EntityDefinition
                {
                    EntityType = obj.Type,
                    Links = new List<LinkDescription>(),
                    Properties = new HashSet<string>()
                };
            if (ed.Links == null)
                ed.Links = new List<LinkDescription>();
            if (ed.Properties == null)
                ed.Properties = new HashSet<string>();
            if (ed.ReferencedBy == null)
                ed.ReferencedBy = new List<LinkDescription>();


            foreach (var prop in obj.Properties ?? new List<Property>())
            {
                ed.Properties.Add(prop.Name);
            }
            foreach (var link in obj.Links ?? new List<Link>())
            {
                var existingLink = ed.Links.FirstOrDefault(x => x.Description == link.LinkDescription.Description);
                if (existingLink == null)
                    ed.Links.Add(link.LinkDescription);
                else
                {
                    existingLink.TargetEntityTypes = existingLink.TargetEntityTypes.Union(link.LinkDescription.TargetEntityTypes).ToArray();
                }
            }
            foreach (var referencingObject in obj.ReferencedFrom)
            {
                ed.ReferencedBy.RemoveAll(x => x.Description == referencingObject.LinkDescription.Description);
                ed.ReferencedBy.Add(referencingObject.LinkDescription);
            }
            var updateTask = entityDefinitions.FindOneAndReplaceAsync<EntityDefinition>(x => x.EntityType == ed.EntityType, ed, new FindOneAndReplaceOptions<EntityDefinition, EntityDefinition> { IsUpsert = true });
            tasks.Add(updateTask);

            Task.WaitAll(tasks.ToArray());
        }

        private IEnumerable<ReportableObject[]> GetReportableObjectsForFolder(OwnedFolder folder, IEnumerable<IConverter> converters)
        {
            List<ReportableObject> objects = new List<ReportableObject>();
            if (this.IsAborted)
                yield break;
            foreach (var converter in converters)
            {
                if (this.IsAborted)
                    yield break;
                string converterTypeName = converter.GetType().GetCustomAttribute<ConvertedTypeAttribute>().ConvertedType.Name;
                lock (syncRoot)
                {
                    message = $"Retrieving {converterTypeName}";
                }
                List<pe.PersistableObject> convertableObjects = folder.SearchByTQL($"=>SUBPARTS:{converterTypeName}");
                lock (syncRoot)
                {
                    message = $"Saving {converterTypeName}";
                }
                yield return convertableObjects.Select(x=>converter.Convert(x)).ToArray();
            }
            //var subfolders = folder.GetAllSubFolders().Cast<OwnedFolder>();
            //foreach (var subfolder in subfolders)
            //{

            //    objects.AddRange(GetReportableObjectsForFolder(subfolder, converters));
            //}
            //var items = folder.Items.Cast<TCObject>();
            //foreach (var item in items)
            //{
            //    if (this.IsAborted)
            //        return new ReportableObject[] { };

            //    var converter = converters.FirstOrDefault(x => x.GetType().GetCustomAttribute<ConvertedTypeAttribute>().ConvertedType == item.GetType());
            //    if (converter == null)
            //        continue;
            //    objects.Add(ConvertObject(item, converter));
            //}
        }

        private ReportableObject ConvertObject(pe.PersistableObject item, IConverter converter)
        {
            return converter.Convert(item);
        }

    }
}

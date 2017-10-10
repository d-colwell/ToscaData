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
using Tricentis.TCCore.BusinessObjects.ExecutionLists;

namespace ToscaExporter.Tasks
{
    class StartMonitorTask : ThreadTask
    {
        private volatile object _syncRoot = new object();
        private Dictionary<string, pe.PersistableObject> objectsToSync = new Dictionary<string, pe.PersistableObject>();
        private DateTime lastChangedDate = DateTime.MinValue;
        Task backgroundThread;
        bool isCancelled = false;
        public override string Name
        {
            get
            {
                return "Experimental";
            }
        }
        protected override void RunInMainThread()
        {
            TCBaseProject project = Object as TCBaseProject;

            var folders = project.Items.Cast<OwnedFolder>();
            foreach (var subfolder in folders)
            {
                RegisterFolderItemsRecursive(subfolder);
            }
            backgroundThread = Task.Run(new Action(SendDataToDatabase));
        }

        private void Subfolder_AspectChangedHandler(pe.PersistableObject changedObject, pe.ChangeAspect changedAspect)
        {
            lock(_syncRoot)
            {
                lastChangedDate = DateTime.Now;
                if (objectsToSync.ContainsKey(changedObject.Surrogate.ToString()))
                    objectsToSync[changedObject.Surrogate.ToString()] = changedObject;
                else
                    objectsToSync.Add(changedObject.Surrogate.ToString(), changedObject);
            }
        }

        protected override void RunInObserverThread()
        {
        }

        private void SendDataToDatabase()
        {
            bool testResult = true;
            while(testResult)
            {
                lock (_syncRoot)
                {
                    testResult = !isCanceled;
                    if (DateTime.Now.Subtract(lastChangedDate).TotalSeconds > 10 && objectsToSync.Count > 0)
                    {
                        MongoDB.Driver.MongoClient client = new MongoDB.Driver.MongoClient();
                        var db = client.GetDatabase("tosca");
                        var snapshots = db.GetCollection<Snapshot>("snapshots");
                        var currentSnapshot = new Snapshot { ID = Guid.NewGuid(), SnapshotDate = DateTime.Now };
                        List<Task> tasks = new List<Task>();
                        foreach (var obj in objectsToSync.Values)
                        {
                            tasks.Add(StoreObjectInDatabase(obj, db, currentSnapshot));
                            if (tasks.Count > 50)
                            {
                                Task.WaitAll(tasks.ToArray());
                                tasks.Clear();
                            }
                        }
                        snapshots.InsertOne(currentSnapshot);
                        //do the things
                        Task.WaitAll(tasks.ToArray());
                        tasks.Clear();
                        objectsToSync.Clear();
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private async Task StoreObjectInDatabase(pe.PersistableObject obj, IMongoDatabase db, Snapshot currentSnapshot)
        {
            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var converterAssembly = System.Reflection.Assembly.GetAssembly(typeof(IConverter));
            var converterTypes = thisAssembly.GetTypes().Where(x => x.CustomAttributes.Any(attr => attr.AttributeType == typeof(ConvertedTypeAttribute))
                                                            && typeof(IConverter).IsAssignableFrom(x)).ToList();
            converterTypes.AddRange(converterAssembly.GetTypes().Where(x => x.CustomAttributes.Any(attr => attr.AttributeType == typeof(ConvertedTypeAttribute))
                                                            && typeof(IConverter).IsAssignableFrom(x)));
            ReportableObject ro = null;
            List<IConverter> converters = new List<IConverter>();
            foreach (var converterType in converterTypes)
            {
                string converterTypeName = converterType.GetCustomAttribute<ConvertedTypeAttribute>().ConvertedType.Name;
                if(converterTypeName == obj.GetType().Name)
                {
                    var converter = (IConverter)Activator.CreateInstance(converterType);
                    ro = converter.Convert(obj);
                    break;
                }
            }
            if (ro == null)
                return;

            var objectCollection = db.GetCollection<ReportableObject>("reportable_object");
            var objectHistory = db.GetCollection<ObjectHistory>("object_history");
            await InsertObject(ro, objectCollection, objectHistory, currentSnapshot);
            await LinkObject(ro, objectCollection);
        }

        public void RegisterFolderItemsRecursive(OwnedFolder folder)
        {
            RegisterObject(folder);
            var items = folder.Items;
            foreach (var item in items)
            {
                if (item is OwnedFolder)
                    RegisterFolderItemsRecursive(item as OwnedFolder);
                else if(item is ExecutionList)
                {
                    var subItems = item.SearchByTQL("=>SUBPARTS:ExecutionEntry");
                    subItems.AddRange(item.SearchByTQL("=>SUBPARTS:ExecutionTestCaseLog"));
                    foreach (var subItem in subItems)
                    {
                        RegisterObject(subItem);
                    }
                }
                else
                {
                    RegisterObject(item);
                }
            }
            items = null;
        }

        public void RegisterObject(pe.PersistableObject po)
        {
            po.AspectChangedHandler -= Subfolder_AspectChangedHandler;
            po.AspectChangedHandler += Subfolder_AspectChangedHandler;

        }
        protected override bool IsAbortable
        {
            get
            {
                return true;
            }
        }
        protected override void FinalizeTask(object retVal, ITaskContext context)
        {
            isCancelled = true;
            base.FinalizeTask(retVal, context);
        }


        #region Helpers to rationalise
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

        #endregion
    }
}

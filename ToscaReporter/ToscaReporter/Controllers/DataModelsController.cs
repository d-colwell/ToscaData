using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MongoDB.Driver;
using ToscaExporter.ObjectModel.ReportingObjects;
using ToscaExporter.ObjectModel.DataObjects;
using ToscaReporter.Controllers.ControllerParameters.DataModels;

namespace ToscaReporter.Controllers
{
    [RoutePrefix("api/DataModels")]
    public class DataModelsController : ApiController
    {
        private readonly MongoDB.Driver.IMongoDatabase _db;
        private readonly MongoDB.Driver.IMongoCollection<DataModel> _models;
        private EntityModelsController _entityController;

        public DataModelsController()
        {
            MongoDB.Driver.MongoClient client = new MongoDB.Driver.MongoClient();
            _db = client.GetDatabase(ConfigurationManager.AppSettings["dbname"]);
            _models = _db.GetCollection<DataModel>("data_models");
            _entityController = new EntityModelsController();
        }
        [HttpGet]
        public async Task<List<DataModel>> GetDataModels()
        {
            var result = await _models.FindAsync(x => true);
            return result.ToList();
        }

        [HttpGet]
        public async Task<DataModel> GetDataModelByID(Guid id)
        {
            var cursor =  await _models.FindAsync(x => x.ID == id);
            var result = await cursor.ToListAsync();
            if (result.Count == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return result.First();
        }

        [HttpPost]
        public async Task<DataModel> PostAsync(NewDataModel newDataModel)
        {
            var existingModels = await _models.FindAsync(x => x.Name == newDataModel.Name);
            if (existingModels.Any())
                throw new HttpResponseException(HttpStatusCode.Conflict);
            var model = new DataModel { ID = Guid.NewGuid(), Name = newDataModel.Name };
            await _models.InsertOneAsync(model);
            return model;
        }

        [HttpPut]
        public async Task<DataModel> PutAsync(DataModel model)
        {
            if (model.ID == Guid.Empty)
                model.ID = Guid.NewGuid();
            var result = await _models.ReplaceOneAsync(x => x.ID == model.ID, model, new UpdateOptions { IsUpsert = true });
            if(result.ModifiedCount == 0)
            { }//No idea what http code to throw here...
            var findModel = await _models.FindAsync(x => x.ID == model.ID);
            return findModel.First();
        }

        [Route("{id}/Link")]
        [HttpPut]
        public async Task<DataModel> PutLinkAsync(Guid id, AddLinkParameter link)
        {
            var model = await GetDataModelByID(id);
            if (model == null)
                return model;
            if (model.EntityLinks == null)
                model.EntityLinks = new List<EntityLink>();
            var existingLink = model.EntityLinks.FirstOrDefault(x=>x.Left.EntityType == link.Left && x.Right.EntityType == link.Right);
            if (existingLink == null)
            {
                var left = await _entityController.GetEntityModel(link.Left);
                var right = await _entityController.GetEntityModel(link.Right);
                if(left == null || right == null)
                {
                    BadRequest();
                    return null;
                }
                existingLink = new EntityLink
                {
                    DescriptiveName = link.DescriptiveName,
                    LinkType = link.LinkType,
                    Left = left,
                    Right = right
                };
                model.EntityLinks.Add(existingLink);
            }
            else
            {
                existingLink.DescriptiveName = link.DescriptiveName;
                existingLink.LinkType = link.LinkType;
            }
            return await PutAsync(model);
        }

        [Route("{id}/Filter")]
        [HttpPut]
        public async Task<DataModel> PutFilterAsync(Guid id, AddFilterParameter filter)
        {
            var model = await GetDataModelByID(id);
            if (model == null)
                return model;
            if (model.Filters == null)
                model.Filters= new List<Filter>();

            var existingEntityFilter = model.Filters.FirstOrDefault(x => x.EntityType == filter.EntityType);

            if (existingEntityFilter == null)
            {
                existingEntityFilter = new Filter
                {
                    EntityType = filter.EntityType
                };
                model.Filters.Add(existingEntityFilter);
            }
            existingEntityFilter.PropertyFilters.RemoveAll(x => x.Property == filter.Filter.Property);
            existingEntityFilter.PropertyFilters.Add(filter.Filter);
            return await PutAsync(model);
        }

    }
}

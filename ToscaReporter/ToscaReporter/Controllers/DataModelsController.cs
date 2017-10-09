using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ToscaReporter.Models;
using MongoDB.Driver;

namespace ToscaReporter.Controllers
{
    [Route("api/DataModels")]
    public class DataModelsController : ApiController
    {
        private readonly MongoDB.Driver.IMongoDatabase _db;
        private readonly MongoDB.Driver.IMongoCollection<DataModel> _models;
        public DataModelsController()
        {
            MongoDB.Driver.MongoClient client = new MongoDB.Driver.MongoClient();
            _db = client.GetDatabase(ConfigurationManager.AppSettings["dbname"]);
            _models = _db.GetCollection<DataModel>("data_models");
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
    }
}

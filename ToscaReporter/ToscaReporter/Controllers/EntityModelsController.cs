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

namespace ToscaReporter.Controllers
{
    [Route("api/EntityModels")]
    public class EntityModelsController : ApiController
    {
        private readonly MongoDB.Driver.IMongoDatabase _db;
        private readonly MongoDB.Driver.IMongoCollection<EntityDefinition> _models;
        public EntityModelsController()
        {
            MongoDB.Driver.MongoClient client = new MongoDB.Driver.MongoClient();
            _db = client.GetDatabase(ConfigurationManager.AppSettings["dbname"]);
            _models = _db.GetCollection<EntityDefinition>("entity_definitions");
        }
        public async Task<List<EntityDefinition>> GetEntityModels()
        {
            var result = await _models.FindAsync(x => true);
            return result.ToList();
        }
        public async Task<EntityDefinition> GetEntityModel(string entityType)
        {
            var resultCursor = await _models.FindAsync(x => x.EntityType == entityType);
            var result = await resultCursor.FirstOrDefaultAsync();
            if (result == null)
                NotFound();
            return result;
        }
    }
}

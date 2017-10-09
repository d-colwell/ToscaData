using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToscaReporter.Models
{
    public class DataModel
    {
        [BsonId]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public List<Entity> Entities { get; set; }
    }
    public class Entity
    {
        public string EntityType { get; set; }
        public string Filter { get; set; }
        public List<Connection> Connections { get; set; }
    }
    public class Connection
    {
        public string LinkName { get; set; }
        public string TerminatingEntityType { get; set; }
    }


}
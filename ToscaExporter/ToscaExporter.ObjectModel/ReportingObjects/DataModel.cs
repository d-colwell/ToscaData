using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.ObjectModel.ReportingObjects
{
    public class DataModel
    {
        [BsonId]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public List<EntityLink> EntityLinks { get; set; } = new List<EntityLink>();
        public List<Filter> Filters { get; set; } = new List<Filter>();
    }
}

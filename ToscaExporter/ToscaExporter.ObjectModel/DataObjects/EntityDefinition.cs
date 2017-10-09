using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.ObjectModel.DataObjects
{
    public class EntityDefinition
    {
        [BsonId]
        public string EntityType { get; set; }

        public List<LinkDescription> Links { get; set; } = new List<LinkDescription>();
        public HashSet<string> Properties { get; set; } = new HashSet<string>();
        public List<LinkDescription> ReferencedBy { get; set; } = new List<LinkDescription>();
    }
}

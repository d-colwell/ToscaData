using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter
{
    public class Snapshot
    {
        [BsonId]
        public Guid ID { get; set; }
        public DateTime SnapshotDate { get; set; }
    }
}

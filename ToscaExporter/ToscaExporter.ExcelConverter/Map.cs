using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.ExcelConverter
{
    public class Map
    {
        [BsonId]
        public string ID { get; set; }

        public int AlternateId { get; set; }
    }
}

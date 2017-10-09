using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.DataObjects
{
    public class ReportableObject
    {
        [BsonId]
        public string ID { get; set; }
        public Link[] Links { get; set; }
        public Property[] Properties { get; set; } 
        public string Type { get; set; }
        public object this[string index]
        {
            get
            {
                if (!Properties.Any(x => x.Name == index))
                    return null;
                else
                    return Properties.First(x => x.Name == index).Value;
            }
        }
        public byte[] Hash { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.ObjectModel.DataObjects
{
    public class LinkDescription
    {
        public string SourceEntity { get; set; }
        public string Description { get; set; }
        public string[] TargetEntityTypes { get; set; }
    }
}

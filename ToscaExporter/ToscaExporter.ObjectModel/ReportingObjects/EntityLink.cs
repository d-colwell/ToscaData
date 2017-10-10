using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.ObjectModel.ReportingObjects
{
    public class EntityLink
    {
        public EntityLink() { }
        public string DescriptiveName { get; set; }
        public DataObjects.EntityDefinition Left { get; set; }
        public DataObjects.EntityDefinition Right { get; set; }
        public LinkType LinkType { get; set; }
    }
}

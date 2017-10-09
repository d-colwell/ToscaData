using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.ObjectModel.DataObjects
{
    public class ObjectHistory
    {
        public Snapshot Snapshot { get; set; }
        public ReportableObject ReportableObject { get; set; }
    }
}

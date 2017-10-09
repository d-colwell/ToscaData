using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.ObjectModel.DataObjects
{
    public class ErrorLog
    {
        public string Error { get; set; }
        public string Stacktrace { get; set; }
        public DateTime DateTime { get; set; }
    }
}

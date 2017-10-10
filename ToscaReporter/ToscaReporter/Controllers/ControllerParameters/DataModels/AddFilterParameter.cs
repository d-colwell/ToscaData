using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToscaExporter.ObjectModel.ReportingObjects;

namespace ToscaReporter.Controllers.ControllerParameters.DataModels
{
    public class AddFilterParameter
    {
        public string EntityType { get; set; }
        public PropertyFilter Filter { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToscaExporter.ObjectModel.ReportingObjects;

namespace ToscaReporter.Controllers.ControllerParameters.DataModels
{
    public class AddLinkParameter
    {
        public Guid Id { get; set; }
        public string DescriptiveName { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public LinkType LinkType { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;
using Tricentis.TCCore.BusinessObjects.ExecutionLists;
using Tricentis.TCCore.BusinessObjects.Testcases;
using Tricentis.TCCore.Persistency;

namespace ToscaExporter.Converter.ConverterImplementations
{
    [ConvertedType(typeof(ExecutionEntry))]
    internal class ExecutionEntryConverter : ConverterBase<ExecutionEntry>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            var obj = base.Convert(toscaObject);
            obj.Type = "Execution Entry";
            return obj;
        }

        protected override void PopulateProperties(ExecutionEntry from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
            properties.Add(new Property { Name = nameof(from.ActualResult), Value = from.ActualResult.ToString() });
            properties.Add(new Property { Name = nameof(from.IsBusinessExecutionEntry), Value = from.IsBusinessExecutionEntry});
            to.Properties = properties;
        }


        protected override void PopulateLinks(ExecutionEntry from, ReportableObject to)
        {
            List<Link> links = new List<Link>();
            var logs = from.ExecutionLogs.ToArray();
            if (logs.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Execution Logs", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Execution Log" } }, LinkedObjects = logs.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

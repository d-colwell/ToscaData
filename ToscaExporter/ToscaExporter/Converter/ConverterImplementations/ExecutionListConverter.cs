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
    [ConvertedType(typeof(ExecutionList))]
    internal class ExecutionListConverter :  ConverterBase<ExecutionList>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Execution List";
            return obj;
        }

        protected override void PopulateProperties(ExecutionList from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
            properties.Add(new Property { Name = nameof(from.NumberOfEntries), Value = from.NumberOfEntries });
            properties.Add(new Property { Name = nameof(from.NumberOfTestCases), Value = from.NumberOfTestCases });
            properties.Add(new Property { Name = nameof(from.NumberOfTestCasesFailed), Value = from.NumberOfTestCasesFailed});
            properties.Add(new Property { Name = nameof(from.NumberOfTestCasesNotExecuted), Value = from.NumberOfTestCasesNotExecuted });
            properties.Add(new Property { Name = nameof(from.NumberOfTestCasesPassed), Value = from.NumberOfTestCasesPassed });
            properties.Add(new Property { Name = nameof(from.NumberOfTestCasesWithUnknownState), Value = from.NumberOfTestCasesWithUnknownState });
            foreach (var prop in from.Properties.ToArray())
            {
                properties.Add(new Property { Name = prop.Name, Value = prop.Value });
            }
            to.Properties = properties;
        }

        protected override void PopulateLinks(ExecutionList from, ReportableObject to)
        {
            List<Link> links = new List<Link>();
            var entries = from.AllExecutionEntries.ToArray();
            var logs = from.ExecutionLogs.ToArray();
            if (entries.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Entries", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Execution Entry" } },
                    LinkedObjects = entries.Select(x => x.Surrogate.ToString()).ToArray() });
            if (logs.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Execution Logs", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Execution Log" } }, LinkedObjects = logs.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

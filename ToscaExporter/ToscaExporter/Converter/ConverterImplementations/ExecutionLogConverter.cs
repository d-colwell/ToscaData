using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;
using Tricentis.TCCore.BusinessObjects.ExecutionLists;
using Tricentis.TCCore.BusinessObjects.ExecutionLists.ExecutionLogs;
using Tricentis.TCCore.BusinessObjects.Testcases;
using Tricentis.TCCore.Persistency;

namespace ToscaExporter.Converter.ConverterImplementations
{
    [ConvertedType(typeof(ExecutionLog))]
    internal class ExecutionLogConverter : ConverterBase<ExecutionLog>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Execution Log";
            return obj;
        }

        protected override void PopulateProperties(ExecutionLog from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
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

        protected override void PopulateLinks(ExecutionLog from, ReportableObject to)
        {
            List<Link> links = new List<Link>();
            var testCaseLogs = from.AllTestCaseLogs.ToArray();
            if (testCaseLogs.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Test Case Logs", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Test Case Execution Log" } }, LinkedObjects = testCaseLogs.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

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
    [ConvertedType(typeof(ExecutionTestCaseLog))]
    internal class ExecutionTestCaseLogConverter : ConverterBase<ExecutionTestCaseLog>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Execution Test Case Log";
            return obj;
        }

        protected override void PopulateProperties(ExecutionTestCaseLog from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
            properties.Add(new Property { Name = nameof(from.Detail), Value = from.Detail });
            properties.Add(new Property { Name = nameof(from.Duration), Value = from.Duration });
            properties.Add(new Property { Name = nameof(from.EndTime), Value = from.EndTime });
            properties.Add(new Property { Name = nameof(from.ExecutedTestCase), Value = from.ExecutedTestCase.Get()?.Surrogate.ToString()});
            properties.Add(new Property { Name = nameof(from.UserName), Value = from.UserName });
            properties.Add(new Property { Name = nameof(from.StartTime), Value = from.StartTime });
            properties.Add(new Property { Name = nameof(from.Result), Value = from.Result.ToString() });
            foreach (var prop in from.GetAllPropertyNames().ToArray())
            {
                properties.Add(new Property { Name = prop, Value = from.GetPropertyValue(prop) });
            }
            to.Properties = properties;
        }

        protected override void PopulateLinks(ExecutionTestCaseLog from, ReportableObject to)
        {
            //issues
            List<Link> links = new List<Link>();
            var issues = from.Issues.ToArray();
            if (issues.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Issues", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Issue" } }, LinkedObjects = issues.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

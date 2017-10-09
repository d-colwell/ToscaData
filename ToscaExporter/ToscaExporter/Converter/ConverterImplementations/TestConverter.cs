using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;
using Tricentis.TCAddIns.XDefinitions.Testcases;
using Tricentis.TCCore.BusinessObjects.Testcases;
using Tricentis.TCCore.Persistency;

namespace ToscaExporter.Converter.ConverterImplementations
{
    [ConvertedType(typeof(TestCase))]
    internal class TestConverter : ConverterBase<TestCase>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Test Case";
            return obj;
        }

        protected override void PopulateProperties(TestCase from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
            properties.Add(new Property { Name = nameof(from.Description), Value = from.Description});
            properties.Add(new Property { Name = nameof(from.IsTemplate), Value = from.IsTemplate});
            properties.Add(new Property { Name = nameof(from.IsOsvItem), Value = from.IsOsvItem });
            string testType = "Automated";
            if(from.ContainsManualItems)
                testType = "Manual";

            properties.Add(new Property { Name = "Test Type", Value = testType });
            foreach (var prop in from.GetAllProperties().ToArray())
            {
                properties.Add(new Property { Name = prop.Name, Value = prop.Value });
            }
            to.Properties = properties;
        }

        protected override void PopulateLinks(TestCase from, ReportableObject to)
        {
            List<Link> links = new List<Link>();
            var referencedBy = from.ReferencedBy.ToArray();
            var executionEntries = from.ExecutionEntries.ToArray();
            var executionLogs = from.ExecutionLogs.ToArray();
            var steps = from.GetAllTestSteps().ToArray();
            var xSteps = from.SearchByTQL("=>SUBPARTS:XTestStep").ToArray();
            if (steps.Length > 0 || xSteps.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Test Steps", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Test Step" } }, LinkedObjects = steps.Select(x => x.Surrogate.ToString()).ToArray().Union(xSteps.Select(x=>x.Surrogate.ToString())).ToArray() });
            if (executionEntries.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Execution Entries", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Execution Entry" } }, LinkedObjects = executionEntries.Select(x => x.Surrogate.ToString()).ToArray() });
            if (executionLogs.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Execution Logs", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Execution Test Case Log" } }, LinkedObjects = executionLogs.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

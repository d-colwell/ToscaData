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
    [ConvertedType(typeof(TestStepValue))]
    internal class TestStepValueConverter : ConverterBase<TestStepValue>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Test Step Value";
            return obj;
        }

        protected override void PopulateProperties(TestStepValue from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
            properties.Add(new Property { Name = nameof(from.Invalidated), Value = from.Invalidated });
            properties.Add(new Property { Name = nameof(from.Value), Value = from.Value });
            properties.Add(new Property { Name = nameof(from.ActionMode), Value = from.ActionMode });
            properties.Add(new Property { Name = nameof(from.Name), Value = from.Name });
            foreach (var prop in from.GetAllPropertyNames().ToArray())
            {
                properties.Add(new Property { Name = prop, Value = from.GetPropertyValue(prop) });
            }
            to.Properties = properties;
        }

        protected override void PopulateLinks(TestStepValue from, ReportableObject to)
        {
            //issues
            List<Link> links = new List<Link>();
            var valueLogs = from.ExecutionLogs.ToArray();
            if (valueLogs.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Execution Test Step Value Logs", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Execution Test Step Value Log" } }, LinkedObjects = valueLogs.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

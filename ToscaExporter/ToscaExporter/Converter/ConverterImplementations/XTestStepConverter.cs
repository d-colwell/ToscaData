using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;
using Tricentis.TCAddIns.XDefinitions.Testcases;
using Tricentis.TCCore.BusinessObjects.ExecutionLists;
using Tricentis.TCCore.BusinessObjects.Testcases;
using Tricentis.TCCore.Persistency;

namespace ToscaExporter.Converter.ConverterImplementations
{
    [ConvertedType(typeof(XTestStep))]
    internal class XTestStepConverter : ConverterBase<XTestStep>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Test Step";
            return obj;
        }

        protected override void PopulateProperties(XTestStep from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
            properties.Add(new Property { Name = nameof(from.Disabled), Value = from.Disabled});
            foreach (var prop in from.GetAllPropertyNames().ToArray())
            {
                properties.Add(new Property { Name = prop, Value = from.GetPropertyValue(prop) });
            }
            to.Properties = properties;
        }

        protected override void PopulateLinks(XTestStep from, ReportableObject to)
        {
            List<Link> links = new List<Link>();
            var logs = from.ExecutionLogs.ToArray();
            var values = from.TestStepValues.ToArray();

            if (logs.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Execution Test Step Logs", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Execution Test Step Log" } }, LinkedObjects = logs.Select(x => x.Surrogate.ToString()).ToArray() });
            if (values.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Test Step Values", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Test Step Value" } }, LinkedObjects = values.Select(x => x.Surrogate.ToString()).ToArray() });

            to.Links = links;
        }
    }
}

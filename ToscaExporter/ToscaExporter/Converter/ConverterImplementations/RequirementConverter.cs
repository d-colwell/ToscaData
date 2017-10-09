using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;
using Tricentis.TCAddIns.RequirementsManagement;
using Tricentis.TCCore.BusinessObjects.ExecutionLists;
using Tricentis.TCCore.BusinessObjects.Testcases;
using Tricentis.TCCore.Persistency;

namespace ToscaExporter.Converter.ConverterImplementations
{
    [ConvertedType(typeof(Requirement))]
    internal class RequirementConverter : ConverterBase<Requirement>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Requirement";
            return obj;
        }

        protected override void PopulateProperties(Requirement from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
            properties.Add(new Property { Name = nameof(from.DamageClass), Value = from.DamageClass });
            properties.Add(new Property { Name = nameof(from.FrequencyClass), Value = from.FrequencyClass});
            properties.Add(new Property { Name = nameof(from.CoverageExecuted), Value = from.CoverageExecuted });
            foreach (var prop in from.GetAllPropertyNames().ToArray())
            {
                properties.Add(new Property { Name = prop, Value = from.GetPropertyValue(prop) });
            }
            to.Properties = properties;
        }

        protected override void PopulateLinks(Requirement from, ReportableObject to)
        {
            List<Link> links = new List<Link>();
            if (from.TotalTestCaseLinkCount > 0)
            {
                var logs = from?.TestCaseLink.Select(x => x?.TestedBy.Get()).ToArray();
                if (logs != null && logs.Length > 0)
                    links.Add(new Link { LinkDescription = new LinkDescription { Description = "Linked Test Cases", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Test Case" } }, LinkedObjects = logs?.Select(x => x?.Surrogate.ToString()).ToArray() });
            }
            to.Links = links;
        }
    }
}

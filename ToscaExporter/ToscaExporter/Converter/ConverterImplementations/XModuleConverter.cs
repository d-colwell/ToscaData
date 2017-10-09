using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;
using Tricentis.TCAddIns.XDefinitions.Modules;
using Tricentis.TCAddIns.XDefinitions.Testcases;
using Tricentis.TCCore.BusinessObjects.ExecutionLists;
using Tricentis.TCCore.BusinessObjects.Testcases;
using Tricentis.TCCore.Persistency;

namespace ToscaExporter.Converter.ConverterImplementations
{
    [ConvertedType(typeof(XModule))]
    internal class XModuleConverter : ConverterBase<XModule>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Module";
            return obj;
        }

        protected override void PopulateProperties(XModule from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
            properties.Add(new Property { Name = "Module Type", Value = "XModule"});

            foreach (var prop in from.GetAllPropertyNames().ToArray())
            {
                properties.Add(new Property { Name = prop, Value = from.GetPropertyValue(prop) });
            }
            foreach(var param in from.XParams)
            {
                properties.Add(new Property { Name = param.Name, Value = param.Value });
            }
            to.Properties = properties;
        }

        protected override void PopulateLinks(XModule from, ReportableObject to)
        {
            List<Link> links = new List<Link>();
            var logs = from.TestSteps.ToArray();
            var attributes = from.AllAttributes.ToArray();

            if (logs.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Test Steps", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Test Step" } }, LinkedObjects = logs.Select(x => x.Surrogate.ToString()).ToArray() });
            if (attributes.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Module Attributes", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Module Attribute" } }, LinkedObjects = attributes.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;
using Tricentis.TCAddIns.XDefinitions.Modules;
using Tricentis.TCAddIns.XDefinitions.Testcases;
using Tricentis.TCCore.BusinessObjects.ExecutionLists;
using Tricentis.TCCore.BusinessObjects.Modules;
using Tricentis.TCCore.BusinessObjects.Testcases;
using Tricentis.TCCore.Persistency;

namespace ToscaExporter.Converter.ConverterImplementations
{
    [ConvertedType(typeof(Module))]
    internal class ModuleConverter : ConverterBase<Module>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Module";
            return obj;
        }

        protected override void PopulateProperties(Module from, ReportableObject to)
        {
            to.ID = from.Surrogate.ToString();
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});
            properties.Add(new Property { Name = "Module Type", Value = "Module"});

            foreach (var prop in from.GetAllPropertyNames().ToArray())
            {
                properties.Add(new Property { Name = prop, Value = from.GetPropertyValue(prop) });
            }
            to.Properties = properties;
        }


        protected override void PopulateLinks(Module from, ReportableObject to)
        {
            List<Link> links = new List<Link>();
            var steps = from.TestSteps.ToArray();
            var attributes = from.ModuleAttributes.ToArray();

            if (steps.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Test Steps", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Test Step" } }, LinkedObjects = steps.Select(x => x.Surrogate.ToString()).ToArray() });
            if (attributes.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Module Attributes", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Module Attribute" } }, LinkedObjects = attributes.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

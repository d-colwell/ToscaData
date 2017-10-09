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
    [ConvertedType(typeof(XModuleAttribute))]
    internal class XModuleAttributeConverter : ConverterBase<XModuleAttribute>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Module Attribute";
            return obj;
        }

        protected override void PopulateProperties(XModuleAttribute from, ReportableObject to)
        {

            #region Basic Details
            to.ID = from.Surrogate.ToString();
            #endregion
            List<Property> properties = new List<Property>();
            properties.Add(new Property { Name = nameof(from.DisplayedName), Value = from.DisplayedName });
            properties.Add(new Property { Name = nameof(from.NodePath), Value = from.NodePath});

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

        protected override void PopulateLinks(XModuleAttribute from, ReportableObject to)
        {
            List<Link> links = new List<Link>();
            //var attributes = from.SearchByTQL("=>SUBPARTS:XModuleAttribute").Cast<XModuleAttribute>().ToArray();

            //if (attributes.Length > 0)
            //    links.Add(new Link { LinkDescription = "Module Attributes", LinkedObjects = attributes.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

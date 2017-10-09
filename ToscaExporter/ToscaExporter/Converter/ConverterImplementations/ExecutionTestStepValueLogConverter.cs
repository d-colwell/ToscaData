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
    [ConvertedType(typeof(ExecutionTestStepValueLog))]
    internal class ExecutionTestStepValueLogConverter : ConverterBase<ExecutionTestStepValueLog>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Execution Step Value Log";
            return obj;
        }

        protected override void PopulateProperties(ExecutionTestStepValueLog from, ReportableObject to)
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
            properties.Add(new Property { Name = nameof(from.AggregatedDescription), Value = from.AggregatedDescription});
            properties.Add(new Property { Name = nameof(from.Invalidated), Value = from.Invalidated });
            properties.Add(new Property { Name = nameof(from.StartTime), Value = from.StartTime });
            properties.Add(new Property { Name = nameof(from.Result), Value = from.Result.ToString() });
            properties.Add(new Property { Name = nameof(from.LogInfo), Value = from.LogInfo });
            properties.Add(new Property { Name = nameof(from.Used), Value = from.Used });
            properties.Add(new Property { Name = nameof(from.UsedValue), Value = from.UsedValue });
            foreach (var prop in from.GetAllPropertyNames().ToArray())
            {
                properties.Add(new Property { Name = prop, Value = from.GetPropertyValue(prop) });
            }
            to.Properties = properties;
        }

        protected override void PopulateLinks(ExecutionTestStepValueLog from, ReportableObject to)
        {
            //issues
            List<Link> links = new List<Link>();
            to.Links = links;
        }
    }
}

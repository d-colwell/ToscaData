﻿using System;
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
    [ConvertedType(typeof(ExecutionTestStepLog))]
    internal class ExecutionTestStepLogConverter : ConverterBase<ExecutionTestStepLog>
    {
        public override ReportableObject Convert(PersistableObject toscaObject)
        {
            ReportableObject obj = base.Convert(toscaObject);
            obj.Type = "Execution Test Step Log";
            return obj;
        }

        protected override void PopulateProperties(ExecutionTestStepLog from, ReportableObject to)
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
            foreach (var prop in from.GetAllPropertyNames().ToArray())
            {
                properties.Add(new Property { Name = prop, Value = from.GetPropertyValue(prop) });
            }
            to.Properties = properties;
        }

        protected override void PopulateLinks(ExecutionTestStepLog from, ReportableObject to)
        {
            //issues
            List<Link> links = new List<Link>();
            var valueLogs = from.TestStepValueLogsInRightOrder.ToArray();
            if (valueLogs.Length > 0)
                links.Add(new Link { LinkDescription = new LinkDescription { Description = "Test Step Value Logs", SourceEntity = to.Type, TargetEntityTypes = new string[] { "Test Step Value Log" } }, LinkedObjects = valueLogs.Select(x => x.Surrogate.ToString()).ToArray() });
            to.Links = links;
        }
    }
}

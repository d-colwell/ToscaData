using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.ObjectModel.ReportingObjects
{
    public class Filter
    {
        public string EntityType { get; set; }
        public List<PropertyFilter> PropertyFilters { get; set; } = new List<PropertyFilter>();
    }

    public class PropertyFilter
    {
        public string Property { get; set; }
        public Operation Operation { get; set; }
        public List<object> Values { get; set; }
    }

    public class Operation
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public OperationNature Nature { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public OperationType Type { get; set; }
    }
    public enum OperationNature
    {
        @is,
        is_not
    }
    public enum OperationType
    {
        equal_to,
        one_of,
        a_match_for
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToscaExporter.Converter
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class ConvertedTypeAttribute : Attribute
    {
        public ConvertedTypeAttribute(Type toscaObjectTypeToConvert)
        {
            this.ConvertedType = toscaObjectTypeToConvert;
        }

        public Type ConvertedType { get; private set; }
    }
}

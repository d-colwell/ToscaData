using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;
using Tricentis.TCCore.Persistency;

namespace ToscaExporter.Converter
{
    public interface IConverter
    {
        ReportableObject Convert(PersistableObject toscaObject);
    }
}

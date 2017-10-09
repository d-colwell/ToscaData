using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.TCCore.Persistency.AddInManager;

namespace ToscaExporter
{
    public class ToscaExporter : TCAddIn
    {
        public override string UniqueName
        {
            get
            {
                return Resources.AddInName;
            }
        }
    }
}

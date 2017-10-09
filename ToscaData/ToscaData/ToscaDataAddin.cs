using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.TCCore.Persistency.AddInManager;

namespace ToscaData
{
    public class ToscaDataAddin : TCAddIn
    {
        public override string UniqueName
        {
            get
            {
                return Resources.Resources.AddinName;
            }
        }
    }
}

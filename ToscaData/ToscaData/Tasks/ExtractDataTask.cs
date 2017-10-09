using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.TCCore.BusinessObjects.Folders;
using Tricentis.TCCore.Persistency.Tasks;

namespace ToscaData.Tasks
{
    public class ExtractDataTask : ThreadTask
    {
        private TCFolder tCFolder;

        public ExtractDataTask(TCFolder tCFolder)
        {
            this.tCFolder = tCFolder;
        }

        public override string Name
        {
            get
            {
                return Resources.Resources.ExtractTaskName;
            }
        }

        protected override void RunInMainThread()
        {
            throw new NotImplementedException();
        }

        protected override void RunInObserverThread()
        {
            throw new NotImplementedException();
        }
    }
}

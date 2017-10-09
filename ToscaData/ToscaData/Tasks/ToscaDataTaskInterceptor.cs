using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.TCCore.BusinessObjects.Folders;
using Tricentis.TCCore.Persistency;
using Tricentis.TCCore.Persistency.AddInManager;

namespace ToscaData.Tasks
{
    class ToscaDataTaskInterceptor : TaskInterceptor
    {
        public override void GetTasks(PersistableObject obj, List<Tricentis.TCCore.Persistency.Task> tasks)
        {
            if (obj is TCFolder)
            {
                var folder = obj as TCFolder;
                if (folder.AttachedFiles.Any(x => x.Name.ToLower().Contains("template")))
                    tasks.Add(new ExtractDataTask(folder));
            }
        }
    }
}

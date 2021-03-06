﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.Tasks;
using Tricentis.TCCore.Base;
using Tricentis.TCCore.BusinessObjects.Folders;
using Tricentis.TCCore.Persistency;
using Tricentis.TCCore.Persistency.AddInManager;

namespace ToscaExporter
{
    public class ExportTaskInterceptor : TaskInterceptor
    {
        public ExportTaskInterceptor(TCObject folder)
        {

        }
        public override void GetTasks(PersistableObject obj, List<Tricentis.TCCore.Persistency.Task> tasks)
        {
            if (obj is TCFolder)
                tasks.Add(new ExporterTask());
            if(obj is TCBaseProject)
                tasks.Add(new StartMonitorTask());
        }
    }
}

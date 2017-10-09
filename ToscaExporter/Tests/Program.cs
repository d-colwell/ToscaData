using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            ToscaExporter.ExcelConverter.ExcelConverter conv = new ToscaExporter.ExcelConverter.ExcelConverter();
            conv.GenerateAccentureReport();

        }
    }
}

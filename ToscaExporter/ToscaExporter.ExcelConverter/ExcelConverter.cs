using MongoDB.Driver;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToscaExporter.ObjectModel.DataObjects;

namespace ToscaExporter.ExcelConverter
{
    public class ExcelConverter
    {
        public void GenerateAccentureReport()
        {
            using (FileStream stream = new FileStream(@"C:\Temp\AccentureReport.xlsx", FileMode.Create))
            {
                using (var package = new ExcelPackage(stream))
                {
                    var wb = package.Workbook;
                    var ws = wb.Worksheets.Add("Test Execution");
                    //TestCaseExecutionId	TestCaseName	Phase	Status	Priority	Cycle	ExecutionPlannedDate	ExecutionActualDate	LoggedOn	LoggedBy

                    MongoDB.Driver.MongoClient client = new MongoDB.Driver.MongoClient();
                    var db = client.GetDatabase("tosca");
                    var reportableObjects = db.GetCollection<ReportableObject>("reportable_objects");
                    #region Test Execution
                    var executionLogs = reportableObjects.FindSync<ReportableObject>(Builders<ReportableObject>.Filter.Eq<string>("Type", "Execution Test Case Log")).ToList();

                    ws.Cells[1, 1].Value = "TestCaseExecutionId";
                    ws.Cells[1, 2].Value = "TestCaseName";
                    ws.Cells[1, 3].Value = "Phase";
                    ws.Cells[1, 4].Value = "Status";
                    ws.Cells[1, 5].Value = "Priority";
                    ws.Cells[1, 6].Value = "Cycle";
                    ws.Cells[1, 7].Value = "ExecutionPlannedDate";
                    ws.Cells[1, 8].Value = "ExecutionActualDate";
                    ws.Cells[1, 9].Value = "LoggedOn";
                    ws.Cells[1, 10].Value = "LoggedBy";
                    ws.Cells[1, 11].Value = "ExecutedTestCase";

                    for (int i = 0; i < executionLogs.Count; i++)
                    {
                        ReportableObject log = executionLogs[i];
                        ws.Cells[i + 2, 1].Value = log.ID;
                        ws.Cells[i + 2, 2].Value = log["DisplayedName"];
                        ws.Cells[i + 2, 3].Value = "Product Test";
                        ws.Cells[i + 2, 4].Value = log["Result"];
                        ws.Cells[i + 2, 5].Value = "Medium";
                        ws.Cells[i + 2, 6].Value = "1";
                        ws.Cells[i + 2, 7].Value = log["EndTime"];
                        ws.Cells[i + 2, 7].Style.Numberformat.Format = "dd-mmm-yyyy";

                        ws.Cells[i + 2, 8].Value = log["EndTime"];
                        ws.Cells[i + 2, 8].Style.Numberformat.Format = "dd-mmm-yyyy";

                        ws.Cells[i + 2, 9].Value = log["StartTime"];
                        ws.Cells[i + 2, 9].Style.Numberformat.Format = "dd-mmm-yyyy";

                        ws.Cells[i + 2, 10].Value = log["UserName"];
                        ws.Cells[i + 2, 11].Value = log["ExecutedTestCase"];
                    }
                    #endregion
                    #region Test Cases
                    var testCases = reportableObjects.Find(x => x.Type == "Test Case").ToList();
                    ws = wb.Worksheets.Add("Test Cases");

                    ws.Cells[1, 1].Value = "TestCaseId";
                    ws.Cells[1, 2].Value = "TestCaseName";
                    ws.Cells[1, 3].Value = "TestCaseDescription";
                    ws.Cells[1, 4].Value = "Phase";
                    ws.Cells[1, 5].Value = "Priority";
                    ws.Cells[1, 6].Value = "Status";
                    ws.Cells[1, 7].Value = "IsAutomated";
                    ws.Cells[1, 8].Value = "CarryForwardFlag";
                    ws.Cells[1, 9].Value = "Cycle";
                    ws.Cells[1, 10].Value = "TestExecutionPlannedDate";
                    ws.Cells[1, 11].Value = "LoggedBy";
                    ws.Cells[1, 12].Value = "LoggedOn";
                    ws.Cells[1, 13].Value = "Requirement";
                    for (int i = 0; i < testCases.Count; i++)
                    {
                        ReportableObject log = testCases[i];
                        var reqFilter = Builders<ReportableObject>.Filter.ElemMatch(x => x.Links, x => x.LinkDescription.Description == "Linked Test Cases" && x.LinkedObjects.Contains(log.ID));
                        var reqs = reportableObjects.Find(reqFilter).ToList();
                        ws.Cells[i + 2, 1].Value = log.ID;
                        ws.Cells[i + 2, 2].Value = log["DisplayedName"];
                        ws.Cells[i + 2, 3].Value = log["Description"];
                        ws.Cells[i + 2, 4].Value = "Product Test";
                        ws.Cells[i + 2, 5].Value = "Medium";
                        ws.Cells[i + 2, 6].Value = "Created";
                        ws.Cells[i + 2, 7].Value = "No";
                        ws.Cells[i + 2, 8].Value = "No";
                        ws.Cells[i + 2, 9].Value = "1";
                        ws.Cells[i + 2, 10].Value = "";
                        ws.Cells[i + 2, 11].Value = "Admin";
                        ws.Cells[i + 2, 12].Value = "";
                        if (reqs.Count > 0)
                            ws.Cells[i + 2, 13].Value = reqs.Select(x => x.ID).Aggregate((x, y) => $"{x},{y}");
                    }

                    var reqWS = wb.Worksheets.Add("Requirements");
                    var allReqs = reportableObjects.Find(x => x.Type == "Requirement").ToList();
                    for (int i = 0; i < allReqs.Count; i++)
                    {
                        var req = allReqs[i];

                        for (int j = 0; j < req.Properties.Count; j++)
                        {
                            if (i == 0)
                            {
                                if (j == 0)
                                    reqWS.Cells[1, 1].Value = "ID";
                                reqWS.Cells[1, j + 2].Value = req.Properties[j].Name;
                            }
                            if (j == 0)
                                reqWS.Cells[i+2, 1].Value = req.ID;
                            reqWS.Cells[i + 2, j + 2].Value = req.Properties[j].Value;
                        }
                    }
                    #endregion
                    package.Save();
                }
            }
        }
    }
}

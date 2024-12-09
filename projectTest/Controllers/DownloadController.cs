using Microsoft.AspNetCore.Mvc;
using projectTest.Models;
using projectTest.ViewModels;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace projectTest.Controllers
{
    public class DownloadController : Controller
    {
        private readonly dbContext _db; // 全域宣告資料庫物件
        private readonly IWebHostEnvironment _hostingEnvironment; // 宣告 IWebHostEnvironment

        public DownloadController(dbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _db = context ?? throw new ArgumentNullException(nameof(context));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        public ActionResult GenerateWordDocument(string? order_id, string? category)
        {
            // 檢查 order_id 是否為空
            if (order_id == null)
            {
                return NotFound();
            }



            if (category == "勞僱型-專任助理"){

                FullTimeOrderViewModel fullTimeOrderViewModel = new FullTimeOrderViewModel
                {
                    FullTimeOrder = _db.FullTime_order.Find(order_id),
                    FullTimeOrderDetails = _db.FullTime_order_detail
                                             .Where(d => d.FullTime_orderNo == order_id)
                                             .ToList()
                };

                if (fullTimeOrderViewModel.FullTimeOrder == null)
                {
                    return NotFound();
                }

                // 計畫數量
                int planCount = fullTimeOrderViewModel.FullTimeOrderDetails.Count;

                // 根據計畫數選擇模板
                string templateFileName = planCount switch
                {
                    1 => "專任人員聘僱申請單_1.docx",
                    2 => "專任人員聘僱申請單_2.docx",
                    3 => "專任人員聘僱申請單_3.docx",
                    4 => "專任人員聘僱申請單_4.docx",
                    5 => "專任人員聘僱申請單_5.docx"
                };
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", templateFileName);

                using (DocX document = DocX.Load(templatePath))
                {
                    if (fullTimeOrderViewModel.FullTimeOrder.Application == "離職")
                    {
                        string ResignDate = fullTimeOrderViewModel.FullTimeOrder.ResignDate;
                        document.ReplaceText("{Application}", fullTimeOrderViewModel.FullTimeOrder.Application + "(自" +
                            ResignDate.Substring(0, 3) + "年" + ResignDate.Substring(3, 2) + "月" +
                            ResignDate.Substring(5, 2) + "日起離職)");
                    }
                    else if (fullTimeOrderViewModel.FullTimeOrder.Application == "薪資變更")
                    {
                        string salaryChangeDate = fullTimeOrderViewModel.FullTimeOrder.SalaryChangeDate;
                        string salary = fullTimeOrderViewModel.FullTimeOrder.Salary;
                        document.ReplaceText("{Application}", fullTimeOrderViewModel.FullTimeOrder.Application + "自" +
                            salaryChangeDate.Substring(0, 3) + "年" + salaryChangeDate.Substring(3, 2) + "月" +
                            salaryChangeDate.Substring(5, 2) + "日起調整薪資)");
                    }
                    else
                    {
                        document.ReplaceText("{Application}", fullTimeOrderViewModel.FullTimeOrder.Application);
                    }

                    
                    document.ReplaceText("{EmployedName}", fullTimeOrderViewModel.FullTimeOrder.EmployedName);
                    document.ReplaceText("{ID}", fullTimeOrderViewModel.FullTimeOrder.ID);
                    document.ReplaceText("{Phone}", fullTimeOrderViewModel.FullTimeOrder.Phone);
                    document.ReplaceText("{JobTitle}", fullTimeOrderViewModel.FullTimeOrder.JobTitle);
                    document.ReplaceText("{SalaryGrade}", fullTimeOrderViewModel.FullTimeOrder.SalaryGrade);
                    document.ReplaceText("{Salary}", fullTimeOrderViewModel.FullTimeOrder.Salary);                    

                    document.ReplaceText("{ES.Y}", fullTimeOrderViewModel.FullTimeOrder.EmployedStartDate.Substring(0, 3));
                    document.ReplaceText("{ES.M}", fullTimeOrderViewModel.FullTimeOrder.EmployedStartDate.Substring(3, 2));
                    document.ReplaceText("{ES.D}", fullTimeOrderViewModel.FullTimeOrder.EmployedStartDate.Substring(5, 2));
                    document.ReplaceText("{EE.Y}", fullTimeOrderViewModel.FullTimeOrder.EmployedEndDate.Substring(0, 3));
                    document.ReplaceText("{EE.M}", fullTimeOrderViewModel.FullTimeOrder.EmployedEndDate.Substring(3, 2));
                    document.ReplaceText("{EE.D}", fullTimeOrderViewModel.FullTimeOrder.EmployedEndDate.Substring(5, 2));

                    document.ReplaceText("{B.Y}", fullTimeOrderViewModel.FullTimeOrder.Birthday.Substring(0, 3));
                    document.ReplaceText("{B.M}", fullTimeOrderViewModel.FullTimeOrder.Birthday.Substring(3, 2));
                    document.ReplaceText("{B.D}", fullTimeOrderViewModel.FullTimeOrder.Birthday.Substring(5, 2));

                    document.ReplaceText("{EducationLevel}", fullTimeOrderViewModel.FullTimeOrder.EducationLevel);

                    


                    
                    // 假設需要根據每個 FullTimeOrderDetail 進行處理，可以用迴圈
                    int index = 1;
                    foreach (var detail in fullTimeOrderViewModel.FullTimeOrderDetails)
                    {

                        document.ReplaceText($"{{PlanName_{index}}}", detail.PlanName ?? "");
                        document.ReplaceText($"{{PlanNo_{index}}}", detail.PlanNo ?? "");
                        document.ReplaceText($"{{CommissionName_{index}}}", detail.CommissionName ?? "");

                        document.ReplaceText($"{{PS.Y_{index}}}", detail.PlanStartDate.Substring(0, 3) ?? "");
                        document.ReplaceText($"{{PS.M_{index}}}", detail.PlanStartDate.Substring(3, 2) ?? "");
                        document.ReplaceText($"{{PS.D_{index}}}", detail.PlanStartDate.Substring(5, 2) ?? "");
                        
                        document.ReplaceText($"{{PE.Y_{index}}}", detail.PlanEndDate.Substring(0, 3) ?? "");
                        document.ReplaceText($"{{PE.M_{index}}}", detail.PlanEndDate.Substring(3, 2) ?? "");
                        document.ReplaceText($"{{PE.D_{index}}}", detail.PlanEndDate.Substring(5, 2) ?? "");
                   
                        index++;
                    }


                    if (fullTimeOrderViewModel.FullTimeOrder.SalaryChangeReason == null)
                    {
                        document.ReplaceText("{SalaryChangeReason}", "");
                    }
                    else
                    {
                        document.ReplaceText("{SalaryChangeReason}", fullTimeOrderViewModel.FullTimeOrder.SalaryChangeReason);
                    }

                    // 將修改後的檔案儲存到伺服器的臨時位置
                    string tempPath = Path.Combine(Path.GetTempPath(), $"FullTimeOrder_{order_id}.docx");
                    document.SaveAs(tempPath);

                    // 將生成的 Word 檔案返回給客戶端下載或預覽
                    byte[] fileBytes = System.IO.File.ReadAllBytes(tempPath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"FullTimeOrder_{order_id}.docx");

                }
            }
            else
            {
                // 設定模板路徑，使用 IWebHostEnvironment 獲取檔案的根目錄
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "獎助生、兼任助理及臨時工申請表.docx");

                // 處理 PartTime_order
                PartTime_order pt_orders = _db.PartTime_order.Find(order_id);

                if (pt_orders == null)
                {
                    return NotFound();
                }

                // 打開 Word 模板文件
                using (DocX document = DocX.Load(templatePath))
                {
                    // 使用 PartTime_order 的資料替換 Word 模板中的佔位符
                    if (pt_orders.Application == "離職")
                    {
                        string ResignDate = pt_orders.ResignDate;
                        document.ReplaceText("{Application}", pt_orders.Application + "(自" +
                            ResignDate.Substring(0, 3) + "年" + ResignDate.Substring(3, 2) + "月" +
                            ResignDate.Substring(5, 2) + "日起離職)");
                    }
                    else if (pt_orders.Application == "薪資變更")
                    {
                        string salaryChangeDate = pt_orders.SalaryChangeDate;
                        string salary = pt_orders.Salary;
                        string salaryCategory = pt_orders.SalaryCategory;
                        document.ReplaceText("{Application}", pt_orders.Application + "(原薪資為" +
                            salaryCategory + " " + salary + "元，自" +
                            salaryChangeDate.Substring(0, 3) + "年" + salaryChangeDate.Substring(3, 2) + "月" +
                            salaryChangeDate.Substring(5, 2) + "日起調整薪資)");
                    }
                    else
                    {
                        document.ReplaceText("{Application}", pt_orders.Application);
                    }


                    document.ReplaceText("{Category}", pt_orders.Category);
                    document.ReplaceText("{PlanNo}", pt_orders.PlanNo);
                    document.ReplaceText("{PlanName}", pt_orders.PlanName);
                    document.ReplaceText("{AccountNo}", pt_orders.AccountNo);
                    document.ReplaceText("{EmployedName}", pt_orders.EmployedName);
                    document.ReplaceText("{Phone}", pt_orders.phone);
                    document.ReplaceText("{Phone}", pt_orders.phone);
                    document.ReplaceText("{ID}", pt_orders.ID);

                    document.ReplaceText("{PS.Y}", pt_orders.PlanStartDate.Substring(0, 3));
                    document.ReplaceText("{PS.M}", pt_orders.PlanStartDate.Substring(3, 2));
                    document.ReplaceText("{PS.D}", pt_orders.PlanStartDate.Substring(5, 2));
                    document.ReplaceText("{PE.Y}", pt_orders.PlanEndDate.Substring(0, 3));
                    document.ReplaceText("{PE.M}", pt_orders.PlanEndDate.Substring(3, 2));
                    document.ReplaceText("{PE.D}", pt_orders.PlanEndDate.Substring(5, 2));

                    document.ReplaceText("{ES.Y}", pt_orders.EmployedStartDate.Substring(0, 3));
                    document.ReplaceText("{ES.M}", pt_orders.EmployedStartDate.Substring(3, 2));
                    document.ReplaceText("{ES.D}", pt_orders.EmployedStartDate.Substring(5, 2));
                    document.ReplaceText("{EE.Y}", pt_orders.EmployedEndDate.Substring(0, 3));
                    document.ReplaceText("{EE.M}", pt_orders.EmployedEndDate.Substring(3, 2));
                    document.ReplaceText("{EE.D}", pt_orders.EmployedEndDate.Substring(5, 2));

                    document.ReplaceText("{B.Y}", pt_orders.Birthday.Substring(0, 3));
                    document.ReplaceText("{B.M}", pt_orders.Birthday.Substring(3, 2));
                    document.ReplaceText("{B.D}", pt_orders.Birthday.Substring(5, 2));


                    document.ReplaceText("{CommissionName}", pt_orders.CommissionName);

                    if (pt_orders.StudyStatus == "他校學生")
                    {
                        string studyStatusMemo = pt_orders.StudyStatusMemo;
                        document.ReplaceText("{StudyStatus}", pt_orders.StudyStatus + " (就讀學校：" +
                            studyStatusMemo.Split("/")[0] + "，系所：" + studyStatusMemo.Split("/")[1] + ")");
                    }
                    else if (pt_orders.StudyStatus == "非在學人員")
                    {
                        document.ReplaceText("{StudyStatus}", pt_orders.StudyStatus + " (" +
                            pt_orders.StudyStatusMemo + ")");
                    }
                    else
                    {
                        document.ReplaceText("{StudyStatus}", pt_orders.StudyStatus);
                    }

                    document.ReplaceText("{EducationLevel}", pt_orders.EducationLevel);

                    document.ReplaceText("{Salary}", pt_orders.SalaryCategory + "，" + pt_orders.Salary + "元整");

                    if (pt_orders.IsTechPerson == "是")
                    {
                        document.ReplaceText("{□Y}", "■");
                        document.ReplaceText("{□F}", "□");
                    }
                    else
                    {
                        document.ReplaceText("{□Y}", "□");
                        document.ReplaceText("{□F}", "■");
                    }



                    // 將修改後的檔案儲存到伺服器的臨時位置
                    string tempPath = Path.Combine(Path.GetTempPath(), $"PartTimeOrder_{order_id}.docx");
                    document.SaveAs(tempPath);

                    // 將生成的 Word 檔案返回給客戶端下載或預覽
                    byte[] fileBytes = System.IO.File.ReadAllBytes(tempPath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"PartTimeOrder_{order_id}.docx");
                }
            }
        }


    }
}

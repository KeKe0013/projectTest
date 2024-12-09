using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using projectTest.Models;
using projectTest.Services;
using projectTest.ViewModels;
using System.Linq;
using X.PagedList;

namespace projectTest.Controllers
{
    public class ManagerController : Controller
    {
        private readonly dbContext _db;//先在全域宣告資料庫物件
        private readonly EmailService _emailService;

        public ManagerController(dbContext context, EmailService emailService)
        {
            _db = context ?? throw new ArgumentNullException(nameof(context));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }


        public IActionResult ManagerIndex(int? isLogin, int? isManager, int? page, bool? newHire, bool? renewal, bool? resignation,bool? salaryChange, bool? fullTimeAssistant, bool? partTimeAssistant, bool? temporaryWorker,
                bool? researchAssistant, bool? teachAssistant, bool? serviceAssistant, string projectName, string projectLeader, string employedPerson, DateTime? searchDate){

            int pageSize = 10; // 每頁顯示的申請單數量
            int pageNumber = page ?? 1; // 如果 page 為 null，則設置為第 1 頁

            var fullTimeOrdersQuery = _db.FullTime_order
                .Where(o => o.Status != "未送出")
                .Select(o => new FullTimeOrderViewModel
                {
                    FullTimeOrder = o,
                    FullTimeOrderDetails = _db.FullTime_order_detail
                                             .Where(d => d.FullTime_orderNo == o.FullTime_orderNo)
                                             .ToList()
                });

            
            var partTimeOrdersQuery = _db.PartTime_order
                .Where(o => o.Status != "未送出");

            
            var selectedCategories = new List<string>();
            if (fullTimeAssistant == true) selectedCategories.Add("勞僱型-專任助理");
            if (partTimeAssistant == true) selectedCategories.Add("勞僱型-兼任助理");
            if (temporaryWorker == true) selectedCategories.Add("勞僱型-臨時工(工讀生)");
            if (researchAssistant == true) selectedCategories.Add("學習型-研究獎助生");
            if (teachAssistant == true) selectedCategories.Add("學習型-教學獎助生");
            if (serviceAssistant == true) selectedCategories.Add("學習型-附服務負擔助學金");

            
            if (newHire == true || renewal == true || resignation == true || salaryChange == true)
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o =>
                    (newHire == true && o.FullTimeOrder.Application == "新聘") ||
                    (renewal == true && o.FullTimeOrder.Application == "續聘") ||
                    (resignation == true && o.FullTimeOrder.Application == "離職") ||
                    (salaryChange == true && o.FullTimeOrder.Application == "薪資變更")
                );

                partTimeOrdersQuery = partTimeOrdersQuery.Where(o =>
                    (newHire == true && o.Application == "新聘") ||
                    (renewal == true && o.Application == "續聘") ||
                    (resignation == true && o.Application == "離職") ||
                    (salaryChange == true && o.Application == "薪資變更")
                );
            }

           
            if (selectedCategories.Any())
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o => selectedCategories.Contains(o.FullTimeOrder.Category));
                partTimeOrdersQuery = partTimeOrdersQuery.Where(o => selectedCategories.Contains(o.Category));
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o =>o.FullTimeOrderDetails.Any(detail => detail.PlanName.Contains(projectName)));
                partTimeOrdersQuery = partTimeOrdersQuery.Where(o => o.PlanName.Contains(projectName));
            }
            
            if (!string.IsNullOrEmpty(projectLeader))
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o =>o.FullTimeOrderDetails.Any(detail => detail.LeaderName.Contains(projectLeader)));
                partTimeOrdersQuery = partTimeOrdersQuery.Where(o => o.LeaderName.Contains(projectLeader));
            }

            if (!string.IsNullOrEmpty(employedPerson))
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o => o.FullTimeOrder.EmployedName.Contains(employedPerson));
                partTimeOrdersQuery = partTimeOrdersQuery.Where(o => o.EmployedName.Contains(employedPerson));
            }

            
            if (searchDate.HasValue)
            {
                var rocYear = searchDate.Value.Year - 1911;
                var searchDateROC = $"{rocYear}{searchDate.Value:MMdd}";

                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o =>
                    string.Compare(searchDateROC, o.FullTimeOrder.EmployedStartDate) >= 0 &&
                    string.Compare(searchDateROC, o.FullTimeOrder.EmployedEndDate) <= 0
                );

                partTimeOrdersQuery = partTimeOrdersQuery.Where(o =>
                    string.Compare(searchDateROC, o.EmployedStartDate) >= 0 &&
                    string.Compare(searchDateROC, o.EmployedEndDate) <= 0
                );
            }

            
            var fullTimeOrders = fullTimeOrdersQuery.ToList();
            var partTimeOrders = partTimeOrdersQuery.ToList();

            var combinedOrders = new List<object>();
            combinedOrders.AddRange(fullTimeOrders);
            combinedOrders.AddRange(partTimeOrders);

            // 按 ApplicationTime 排序
            var sortedCombinedOrders = combinedOrders.OrderBy(o =>
            {
                if (o is FullTimeOrderViewModel fullTimeOrder)
                {
                    return fullTimeOrder.FullTimeOrder.ApplicationTime;
                }
                else if (o is PartTime_order partTimeOrder)
                {
                    return partTimeOrder.ApplicationTime;
                }
                return DateTime.MaxValue;
            }).ToList();

            // 使用 X.PagedList 進行分頁
            var pagedOrders = sortedCombinedOrders.ToPagedList(pageNumber, pageSize);

            // 保存篩選條件至 ViewData
            ViewData["NewHire"] = newHire;
            ViewData["Renewal"] = renewal;
            ViewData["Resignation"] = resignation;
            ViewData["SalaryChange"] = salaryChange;
            ViewData["FullTimeAssistant"] = fullTimeAssistant;
            ViewData["PartTimeAssistant"] = partTimeAssistant;
            ViewData["TemporaryWorker"] = temporaryWorker;
            ViewData["ResearchAssistant"] = researchAssistant;
            ViewData["TeachAssistant"] = teachAssistant;
            ViewData["ServiceAssistant"] = serviceAssistant;
            ViewData["ProjectName"] = projectName;
            ViewData["ProjectLeader"] = projectLeader;
            ViewData["EmployedPerson"] = employedPerson;
            ViewData["SearchDate"] = searchDate;

            return View(pagedOrders);
        }



        public IActionResult ManagerReview(int? isLogin, int? isManager, string? order_id, string? category)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            if (order_id == null)
            {
                return NotFound();
            }

            // 根據 category 決定使用哪一個訂單類型
            if (category == "勞僱型-專任助理")
            {
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

                return View("FullTime_Review", fullTimeOrderViewModel);
            }
            else
            {
                PartTime_order pt_orders = _db.PartTime_order.Find(order_id);

                if (pt_orders == null)
                {
                    return NotFound();
                }

                return View("PartTime_Review", pt_orders);
            }
        }


        public IActionResult ExportToExcel(bool? newHire, bool? renewal, bool? resignation, bool? salaryChange,
                                   bool? fullTimeAssistant, bool? partTimeAssistant, bool? temporaryWorker,
                                   bool? researchAssistant, bool? teachAssistant, bool? serviceAssistant,
                                   string projectName, string projectLeader, string employedPerson, DateTime? searchDate)
        {
            // 初始化查詢
            var fullTimeOrdersQuery = _db.FullTime_order
                .Where(o => o.Status != "未送出")
                .Select(o => new
                {
                    o.Application,
                    o.Category,
                    o.ApplicantName,
                    o.EmployedName,
                    o.EmployedStartDate,
                    o.EmployedEndDate,
                    o.FullTime_orderNo
                });

            var partTimeOrdersQuery = _db.PartTime_order
                .Where(o => o.Status != "未送出")
                .Select(o => new
                {
                    o.Application,
                    o.Category,
                    o.ApplicantName,
                    o.EmployedName,
                    o.EmployedStartDate,
                    o.EmployedEndDate,
                    o.PartTime_orderNo
                });

            // 根據申請事項篩選
            if (newHire == true || renewal == true || resignation == true || salaryChange == true)
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o =>
                    (newHire == true && o.Application == "新聘") ||
                    (renewal == true && o.Application == "續聘") ||
                    (resignation == true && o.Application == "離職") ||
                    (salaryChange == true && o.Application == "薪資變更")
                );

                partTimeOrdersQuery = partTimeOrdersQuery.Where(o =>
                    (newHire == true && o.Application == "新聘") ||
                    (renewal == true && o.Application == "續聘") ||
                    (resignation == true && o.Application == "離職") ||
                    (salaryChange == true && o.Application == "薪資變更")
                );
            }

            // 根據聘僱類別篩選
            var selectedCategories = new List<string>();
            if (fullTimeAssistant == true) selectedCategories.Add("勞僱型-專任助理");
            if (partTimeAssistant == true) selectedCategories.Add("勞僱型-兼任助理");
            if (temporaryWorker == true) selectedCategories.Add("勞僱型-臨時工(工讀生)");
            if (researchAssistant == true) selectedCategories.Add("學習型-研究獎助生");
            if (teachAssistant == true) selectedCategories.Add("學習型-教學獎助生");
            if (serviceAssistant == true) selectedCategories.Add("學習型-附服務負擔助學金");

            if (selectedCategories.Any())
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o => selectedCategories.Contains(o.Category));
                partTimeOrdersQuery = partTimeOrdersQuery.Where(o => selectedCategories.Contains(o.Category));
            }

            // 根據其他條件篩選
            if (!string.IsNullOrEmpty(projectName))
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o => o.ApplicantName.Contains(projectName));
                partTimeOrdersQuery = partTimeOrdersQuery.Where(o => o.ApplicantName.Contains(projectName));
            }

            if (!string.IsNullOrEmpty(projectLeader))
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o => o.EmployedName.Contains(projectLeader));
                partTimeOrdersQuery = partTimeOrdersQuery.Where(o => o.EmployedName.Contains(projectLeader));
            }

            if (!string.IsNullOrEmpty(employedPerson))
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o => o.EmployedName.Contains(employedPerson));
                partTimeOrdersQuery = partTimeOrdersQuery.Where(o => o.EmployedName.Contains(employedPerson));
            }

            if (searchDate.HasValue)
            {
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o =>
                    string.Compare(o.EmployedStartDate, searchDate.Value.ToString("yyyyMMdd")) <= 0 &&
                    string.Compare(o.EmployedEndDate, searchDate.Value.ToString("yyyyMMdd")) >= 0
                );

                partTimeOrdersQuery = partTimeOrdersQuery.Where(o =>
                    string.Compare(o.EmployedStartDate, searchDate.Value.ToString("yyyyMMdd")) <= 0 &&
                    string.Compare(o.EmployedEndDate, searchDate.Value.ToString("yyyyMMdd")) >= 0
                );
            }

            // 獲取篩選後的結果
            var fullTimeOrders = fullTimeOrdersQuery.ToList();
            var partTimeOrders = partTimeOrdersQuery.ToList();

            var allOrders = fullTimeOrders.Select(o => new
            {
                o.Application,
                o.Category,
                o.ApplicantName,
                o.EmployedName,
                o.EmployedStartDate,
                o.EmployedEndDate,
                OrderNo = o.FullTime_orderNo // 統一名稱為 OrderNo
            })
            .Concat(partTimeOrders.Select(o => new
            {
                o.Application,
                o.Category,
                o.ApplicantName,
                o.EmployedName,
                o.EmployedStartDate,
                o.EmployedEndDate,
                OrderNo = o.PartTime_orderNo // 同樣使用 OrderNo
            }))
            .ToList();

            Console.WriteLine($"AllOrders count: {allOrders.Count}");
            foreach (var order in allOrders)
            {
                Console.WriteLine($"Order: {order.OrderNo}, {order.Application}, {order.Category}");
            }

            // 按申請事項和聘僱類別分類
            var groupedOrders = allOrders
                .GroupBy(o => $"{o.Category.Substring(0, 3)}-{o.Application}")
                .ToDictionary(g => g.Key, g => g.ToList());

            var zipStream = new MemoryStream(); // 用於壓縮多個檔案
            using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                foreach (var group in groupedOrders)
                {
                    // 建立 Excel 檔案
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Orders");

                        // 設置標題行
                        worksheet.Cells[1, 1].Value = "訂單編號";
                        worksheet.Cells[1, 2].Value = "申請事項";
                        worksheet.Cells[1, 3].Value = "聘僱類別";
                        worksheet.Cells[1, 4].Value = "聘僱者";
                        worksheet.Cells[1, 5].Value = "被聘僱者";
                        worksheet.Cells[1, 6].Value = "聘僱開始時間";
                        worksheet.Cells[1, 7].Value = "聘僱結束時間";

                        // 填充資料
                        int row = 2;
                        foreach (var order in group.Value)
                        {
                            worksheet.Cells[row, 1].Value = order.OrderNo;
                            worksheet.Cells[row, 2].Value = order.Application;
                            worksheet.Cells[row, 3].Value = order.Category;
                            worksheet.Cells[row, 4].Value = order.ApplicantName;
                            worksheet.Cells[row, 5].Value = order.EmployedName;
                            worksheet.Cells[row, 6].Value = order.EmployedStartDate;
                            worksheet.Cells[row, 7].Value = order.EmployedEndDate;
                            row++;
                        }

                        // 自動調整欄寬
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // 將 Excel 檔案轉換為 byte array
                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var fileBytes = stream.ToArray();

                        // 新增至壓縮檔案
                        var fileName = $"{group.Key}-{DateTime.Now:yyyyMMdd}.xlsx";
                        var zipEntry = archive.CreateEntry(fileName);
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            zipEntryStream.Write(fileBytes, 0, fileBytes.Length);
                        }
                    }
                }
            }

            // 返回壓縮檔案
            zipStream.Position = 0;
            return File(zipStream.ToArray(), "application/zip", $"FilteredOrders-{DateTime.Now:yyyyMMdd}.zip");
        }



        public IActionResult DeleteOrder(int? isLogin, int? isManager, string? order_id, string? category)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            if (category == "勞僱型-專任助理")
            {
                var order = _db.FullTime_order.Find(order_id);
                if (order == null) return NotFound();

                order.Status = "已撤銷";
                _db.SaveChanges();

                return RedirectToAction("ManagerIndex");
            }
            else
            {
                var pt_order = _db.PartTime_order.Find(order_id);
                if (pt_order == null) return NotFound();

                pt_order.Status = "已撤銷";
                _db.SaveChanges();

                return RedirectToAction("ManagerIndex");
            }
        }

        public IActionResult ApproveOrder(int? isLogin, int? isManager, string? order_id, string? category)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            string ApplicantName, Application, EmployedName, CommissionName, PlanName;


            if (category == "勞僱型-專任助理")
            {
                FullTimeOrderViewModel order = new FullTimeOrderViewModel
                {
                    FullTimeOrder = _db.FullTime_order.Find(order_id),
                    FullTimeOrderDetails = _db.FullTime_order_detail
                                             .Where(d => d.FullTime_orderNo == order_id)
                                             .ToList()
                };

                if (order.FullTimeOrder == null)
                {
                    return NotFound();
                }
                ApplicantName = order.FullTimeOrder.ApplicantName;
                Application = order.FullTimeOrder.Application;
                EmployedName = order.FullTimeOrder.EmployedName;



                CommissionName = string.Join("/", _db.FullTime_order_detail
                                                      .Where(o => o.FullTime_orderNo == order_id)
                                                      .Select(o => o.CommissionName)
                                                      .ToList());

                PlanName = string.Join("/", _db.FullTime_order_detail
                                               .Where(o => o.FullTime_orderNo == order_id)
                                               .Select(o => o.PlanName)
                                               .ToList());

                order.FullTimeOrder.Status = "已通過";
                _db.SaveChanges();


            }
            else
            {
                var order = _db.PartTime_order.Find(order_id);
                if (order == null) return NotFound();

                ApplicantName = order.LeaderName;
                Application = order.Application;
                EmployedName = order.EmployedName;
                CommissionName = order.CommissionName;
                PlanName = order.PlanName;


                order.Status = "已通過";
                _db.SaveChanges();
            }


            var emailBody = $@"
                <p>{ApplicantName}老師惠鑒：</p>
                <p>您所申請專任助理、獎助生、兼任助理及臨時工申請案已審核通過。</p>
                <p>以下為申請資料：</p>
                <ul>
                    <li>申請項目：{category}{Application}</li>
                    <li>約用人員姓名：{EmployedName}</li>
                    <li>計畫補助單位：{CommissionName}</li>
                    <li>計畫名稱：{PlanName}</li>
                </ul>
                <p>如有任何問題，請洽研究發展處承辦人：07-5919101（非國家科學及技術委員會計畫）或 07-5919102（國家科學及技術委員會計畫），謝謝。</p>
                <p>注意︰本郵件是由系統自動產生與發送，請勿直接回覆。</p>
                <p>---------------------------------</p>
                <p>研究發展處 敬上</p>
            ";

            // 使用注入的 EmailService 寄送郵件
            var emailSent = _emailService.SendEmail(
                "國立高雄大學研發處",
                "rd@nuk.edu.tw",
                "",
                "a1103353@mail.nuk.edu.tw",
                "專任助理、獎助生、兼任助理及臨時工申請案審核通過通知(系統自動產生與發送，請勿直接回覆)",
                "",
                emailBody
            );

            if (!emailSent)
            {
                TempData["Error"] = "郵件發送失敗！";
            }


            return RedirectToAction("ManagerIndex");
        }

        public IActionResult RejectOrder(int? isLogin, int? isManager, string? order_id, string? category, string rejectReason)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;


            string ApplicantName, Application, EmployedName, CommissionName, PlanName;


            if (category == "勞僱型-專任助理")
            {
                FullTimeOrderViewModel order = new FullTimeOrderViewModel
                {
                    FullTimeOrder = _db.FullTime_order.Find(order_id),
                    FullTimeOrderDetails = _db.FullTime_order_detail
                                             .Where(d => d.FullTime_orderNo == order_id)
                                             .ToList()
                };

                if (order.FullTimeOrder == null)
                {
                    return NotFound();
                }
                ApplicantName = order.FullTimeOrder.ApplicantName;
                Application = order.FullTimeOrder.Application;
                EmployedName = order.FullTimeOrder.EmployedName;

                CommissionName = string.Join("/", _db.FullTime_order_detail
                                                      .Where(o => o.FullTime_orderNo == order_id)
                                                      .Select(o => o.CommissionName)
                                                      .ToList());

                PlanName = string.Join("/", _db.FullTime_order_detail
                                               .Where(o => o.FullTime_orderNo == order_id)
                                               .Select(o => o.PlanName)
                                               .ToList());

                order.FullTimeOrder.Status = "已退件";
                _db.SaveChanges();


            }
            else
            {
                var order = _db.PartTime_order.Find(order_id);
                if (order == null) return NotFound();

                ApplicantName = order.LeaderName;
                Application = order.Application;
                EmployedName = order.EmployedName;
                CommissionName = order.CommissionName;
                PlanName = order.PlanName;


                order.Status = "已退件";
                _db.SaveChanges();
            }


            var emailBody = $@"
                <p>敬啟者：</p>
                <p>專任助理、獎助生、兼任助理及臨時工申請案已退還申請人，請撥冗至系統修正申請案內容再送出。</p>                
                <ul>
                    <li>申請項目：{category}{Application}</li>
                    <li>約用人員姓名：{EmployedName}</li>
                    <li>計畫補助單位：{CommissionName}</li>
                    <li>計畫名稱：{PlanName}</li>
                    <li>退件原因：{rejectReason}</li>
                </ul>
                <p>如有任何問題，請洽研究發展處承辦人：07-5919101（非國家科學及技術委員會計畫）或 07-5919102（國家科學及技術委員會計畫），謝謝。</p>
                <p>注意︰本郵件是由系統自動產生與發送，請勿直接回覆。</p>
                <p>---------------------------------</p>
                <p>研究發展處 敬上</p>
            ";


            // 使用注入的 EmailService 寄送郵件
            var emailSent = _emailService.SendEmail(
                "國立高雄大學研發處",
                "rd@nuk.edu.tw",
                "",
                "a1103353@mail.nuk.edu.tw",
                "專任助理、獎助生、兼任助理及臨時工申請案退件通知(系統自動產生與發送，請勿直接回覆)",
                "",
                emailBody
            );

            if (!emailSent)
            {
                TempData["Error"] = "郵件發送失敗！";
            }


            return RedirectToAction("ManagerIndex");
        }

    }
}

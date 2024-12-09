using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectTest.Models;
using projectTest.ViewModels;
using X.PagedList;

namespace projectTest.Controllers
{
    public class ResignationController : Controller
    {
                
        private readonly dbContext _db;//先在全域宣告資料庫物件

        public ResignationController(dbContext context) => _db = context ?? throw new ArgumentNullException(nameof(context));
        public IActionResult Resignation_Index(int? isLogin, int? isManager, int? page, bool? fullTimeAssistant, bool? partTimeAssistant, bool? temporaryWorker, bool? researchAssistant, bool? teachAssistant, bool? serviceAssistant, string projectNumber, string projectLeader, string employedPerson, DateTime? searchDate)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var fullTimeOrders = _db.FullTime_order.Where(o => o.Status == "已通過" && o.Application == "新聘").AsQueryable();
            var partTimeOrders = _db.PartTime_order.Where(o => o.Status == "已通過" && o.Application == "新聘").AsQueryable();

            var selectedCategories = new List<string>();

            if (fullTimeAssistant == true) selectedCategories.Add("勞僱型-專任助理");
            if (partTimeAssistant == true) selectedCategories.Add("勞僱型-兼任助理");
            if (temporaryWorker == true) selectedCategories.Add("勞僱型-臨時工(工讀生)");
            if (researchAssistant == true) selectedCategories.Add("學習型-研究獎助生");
            if (teachAssistant == true) selectedCategories.Add("學習型-教學獎助生");
            if (serviceAssistant == true) selectedCategories.Add("學習型-附服務負擔助學金");


            // Further filter based on selected categories
            if (selectedCategories.Any())
            {
                fullTimeOrders = fullTimeOrders.Where(o => selectedCategories.Contains(o.Category));
                partTimeOrders = partTimeOrders.Where(o => selectedCategories.Contains(o.Category));
            }

            if (!string.IsNullOrEmpty(employedPerson))
            {
                fullTimeOrders = fullTimeOrders.Where(o => o.EmployedName.Contains(employedPerson));
                partTimeOrders = partTimeOrders.Where(o => o.EmployedName.Contains(employedPerson));
            }

            // 如果有查詢日期，將其轉換為民國年格式的字串
            if (searchDate.HasValue)
            {
                var rocYear = searchDate.Value.Year - 1911;
                var searchDateROC = $"{rocYear}{searchDate.Value:MMdd}"; // 將查詢日期轉換成民國年格式的字串

                fullTimeOrders = fullTimeOrders.Where(o =>
                    string.Compare(searchDateROC, o.EmployedStartDate) >= 0 && string.Compare(searchDateROC, o.EmployedEndDate) <= 0
                );

                partTimeOrders = partTimeOrders.Where(o =>
                    string.Compare(searchDateROC, o.EmployedStartDate) >= 0 && string.Compare(searchDateROC, o.EmployedEndDate) <= 0
                );
            }


            // 將篩選結果合併
            var combinedOrders = new List<object>();
            combinedOrders.AddRange(fullTimeOrders.Select(o => new FullTimeOrderViewModel
            {
                FullTimeOrder = o,
                FullTimeOrderDetails = _db.FullTime_order_detail
                                 .Where(d => d.FullTime_orderNo == o.FullTime_orderNo)
                                 .ToList()
            }).ToList());

            combinedOrders.AddRange(partTimeOrders.ToList());

            // 按照 ApplicationTime 排序
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

            var pagedOrders = sortedCombinedOrders.ToPagedList(pageNumber, pageSize);

            // 保存篩選條件
            ViewData["FullTimeAssistant"] = fullTimeAssistant;
            ViewData["PartTimeAssistant"] = partTimeAssistant;
            ViewData["TemporaryWorker"] = temporaryWorker;
            ViewData["ResearchAssistant"] = researchAssistant;
            ViewData["TeachAssistant"] = teachAssistant;
            ViewData["ServiceAssistant"] = serviceAssistant;

            ViewData["ProjectNumber"] = projectNumber;
            ViewData["ProjectLeader"] = projectLeader;
            ViewData["EmployedPerson"] = employedPerson;
            ViewData["SearchDate"] = searchDate;

            return View(pagedOrders);
        }

        public IActionResult Resignation(int? isLogin, int? isManager, string? order_id, string? category)
        {
            ViewData["isManager"] = isManager;
            ViewData["isLogin"] = isLogin;

            if (order_id == null)
            {
                return NotFound();
            }

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

                return View("FullTime_Resignation", fullTimeOrderViewModel);
            }
            else
            {
                PartTime_order pt_orders = _db.PartTime_order.Find(order_id);

                if (pt_orders == null)
                {
                    return NotFound();
                }

                return View("PartTime_Resignation", pt_orders);
            }

        }
        [HttpPost]
        public async Task<IActionResult> PartTime_Resignation(string originalOrderId, DateTime ResignDate, string? isLogin, string? isManager,string? category)
        {
            try
            {
                int order_taiwanYear = DateTime.Now.Year - 1911;
                string type = category.StartsWith("勞僱型") ? "01" : category.StartsWith("學習型") ? "02" : "00";
                if (type == "00")
                {
                    ModelState.AddModelError("", "無法辨識的類別。請確認類別是否正確。");
                    return View(originalOrderId);
                }
                int maxSerial = _db.FullTime_order
                    .Where(o => o.FullTime_orderNo.ToString().StartsWith(order_taiwanYear.ToString() + type))
                    .AsEnumerable()
                    .Select(o => int.Parse(o.FullTime_orderNo.ToString().Substring(5)))
                    .DefaultIfEmpty(0)
                    .Max();

                maxSerial = Math.Max(maxSerial, _db.PartTime_order
                    .Where(o => o.PartTime_orderNo.ToString().StartsWith(order_taiwanYear.ToString() + type))
                    .AsEnumerable()
                    .Select(o => int.Parse(o.PartTime_orderNo.ToString().Substring(5)))
                    .DefaultIfEmpty(0)
                    .Max());


                string newOrderID = $"{order_taiwanYear:D3}{type}{(maxSerial + 1):D5}";

                // 取得原始訂單
                var originalOrder = await _db.PartTime_order.FindAsync(originalOrderId);
                if (originalOrder == null)
                {
                    return NotFound();
                }


                string old_employedEndDate = originalOrder.EmployedEndDate;
                string resignDate = ResignDate.ToTaiwanDate();
                int taiwanYear = int.Parse(old_employedEndDate.Substring(0, 3)) + 1911;
                string westernDate = taiwanYear + old_employedEndDate.Substring(3);

                DateTime oldEmployedEndDateDateTime = DateTime.ParseExact(westernDate, "yyyyMMdd", null);
                DateTime newResignDate = ResignDate;

                if (newResignDate != oldEmployedEndDateDateTime.AddDays(1))
                {
                    ModelState.AddModelError("ResignDate", "離職日期必須是聘期結束日期的隔天。");
                }

                if (ModelState.IsValid)
                {
                    originalOrder.isApplyResignation = "1";
                    // 創建新的訂單物件
                    var newOrder = new PartTime_order
                    {
                        PartTime_orderNo = newOrderID,
                        ResignDate = resignDate,
                        Application = "離職",

                        // 複製其他欄位
                        ApplicantName = originalOrder.ApplicantName,
                        ApplicationOffice = originalOrder.ApplicationOffice,
                        LeaderName = originalOrder.LeaderName,
                        Category = originalOrder.Category,
                        PlanName = originalOrder.PlanName,
                        AccountNo = originalOrder.AccountNo,
                        PlanNo = originalOrder.PlanNo,
                        CommissionName = originalOrder.CommissionName,
                        PlanStartDate = originalOrder.PlanStartDate,
                        PlanEndDate = originalOrder.PlanEndDate,
                        StudyStatus = originalOrder.StudyStatus,
                        StudyStatusMemo = originalOrder.StudyStatusMemo,
                        EmployedName = originalOrder.EmployedName,
                        EmployedStartDate = originalOrder.EmployedStartDate,
                        EmployedEndDate = originalOrder.EmployedEndDate,
                        ID = originalOrder.ID,
                        Birthday = originalOrder.Birthday,
                        phone = originalOrder.phone,
                        EducationLevel = originalOrder.EducationLevel,
                        SalaryCategory = originalOrder.SalaryCategory,
                        Salary = originalOrder.Salary,
                        IsTechPerson = originalOrder.IsTechPerson,
                        projtype = originalOrder.projtype,

                        // 設定新申請單的狀態
                        Status = "待審核",
                        ApplicationTime = DateTime.Now
                    };

                    // 將新訂單加入資料庫
                    await _db.PartTime_order.AddAsync(newOrder);
                    await _db.SaveChangesAsync();

                    // 重新導向至適當頁面
                    return RedirectToAction("UserIndex", "User", new { isLogin = isLogin, isManager = isManager });


                }
                ViewData["isManager"] = isManager;
                ViewData["isLogin"] = isLogin;
                ViewData["category"] = category;
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View("PartTime_Resignation", originalOrder);
            }
            catch (Exception ex)
            {

                // 將錯誤訊息顯示於視圖
                ModelState.AddModelError("", $"離職申請失敗，請稍後再試。錯誤訊息：{ex.Message}");
                return View("PartTime_Resignation", await _db.PartTime_order.FindAsync(originalOrderId));
            }
        }

        public async Task<IActionResult> FullTime_Resignation(string originalOrderId, DateTime ResignDate, string? isLogin, string? isManager)
        {
            try
            {
                int order_taiwanYear = DateTime.Now.Year - 1911;
                string type = "01";

                int maxSerial = _db.FullTime_order
                    .Where(o => o.FullTime_orderNo.ToString().StartsWith(order_taiwanYear.ToString() + type))
                    .AsEnumerable()
                    .Select(o => int.Parse(o.FullTime_orderNo.ToString().Substring(5)))
                    .DefaultIfEmpty(0)
                    .Max();

                maxSerial = Math.Max(maxSerial, _db.PartTime_order
                    .Where(o => o.PartTime_orderNo.ToString().StartsWith(order_taiwanYear.ToString() + type))
                    .AsEnumerable()
                    .Select(o => int.Parse(o.PartTime_orderNo.ToString().Substring(5)))
                    .DefaultIfEmpty(0)
                    .Max());


                string newOrderID = $"{order_taiwanYear:D3}{type}{(maxSerial + 1):D5}";

                // 取得原始訂單
                var originalOrder = await _db.FullTime_order
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.FullTime_orderNo == originalOrderId);


                string old_employedEndDate = originalOrder.EmployedEndDate;
                string resignDate = ResignDate.ToTaiwanDate();
                int taiwanYear = int.Parse(old_employedEndDate.Substring(0, 3)) + 1911;
                string westernDate = taiwanYear + old_employedEndDate.Substring(3);

                DateTime oldEmployedEndDateDateTime = DateTime.ParseExact(westernDate, "yyyyMMdd", null);
                DateTime newResignDate = ResignDate;

                if (newResignDate != oldEmployedEndDateDateTime.AddDays(1))
                {
                    ModelState.AddModelError("FullTimeOrder.ResignDate", "離職日期必須是聘期結束日期的隔天。");
                }


                var viewModel = new FullTimeOrderViewModel
                {
                    FullTimeOrder = originalOrder,
                    FullTimeOrderDetails = originalOrder.OrderDetails?.ToList() ?? new List<FullTime_order_detail>()
                };

                if (ModelState.IsValid)
                {
                    originalOrder.isApplyResignation = "1";
                    // 創建新的 FullTime_order 訂單
                    var newOrder = new Models.FullTime_order
                    {
                        FullTime_orderNo = newOrderID,
                        ResignDate = resignDate,
                        Application = "離職",

                        // 複製原始訂單欄位
                        ApplicantName = originalOrder.ApplicantName,
                        ApplicationOffice = originalOrder.ApplicationOffice,
                        Category = originalOrder.Category,
                        EmployedName = originalOrder.EmployedName,
                        EmployedStartDate = originalOrder.EmployedStartDate,
                        EmployedEndDate = originalOrder.EmployedEndDate,
                        ID = originalOrder.ID,
                        Birthday = originalOrder.Birthday,
                        Phone = originalOrder.Phone,
                        EducationLevel = originalOrder.EducationLevel,
                        JobTitle = originalOrder.JobTitle,
                        SalaryGrade = originalOrder.SalaryGrade,
                        Salary = originalOrder.Salary,
                        SalaryChangeReason = originalOrder.SalaryChangeReason,


                        // 設定新申請單狀態
                        Status = "待審核",
                        ApplicationTime = DateTime.Now
                    };

                    // 將新訂單加入資料庫
                    await _db.FullTime_order.AddAsync(newOrder);
                    await _db.SaveChangesAsync();




                    // 為每個計畫新增 FullTime_order_detail 資料
                    foreach (var detail in viewModel.FullTimeOrderDetails)
                    {
                        var newDetail = new Models.FullTime_order_detail
                        {
                            FullTime_orderNo = newOrder.FullTime_orderNo, // 關聯到新訂單
                            PlanName = detail.PlanName,
                            PlanNo = detail.PlanNo,
                            AccountNo = detail.AccountNo,
                            LeaderName = detail.LeaderName,
                            CommissionName = detail.CommissionName,
                            PlanStartDate = detail.PlanStartDate,
                            PlanEndDate = detail.PlanEndDate,
                            projtype = detail.projtype
                            
                        };

                        await _db.FullTime_order_detail.AddAsync(newDetail);
                    }

                    await _db.SaveChangesAsync();

                    // 重新導向至適當頁面
                    return RedirectToAction("UserIndex", "User", new { isLogin = isLogin, isManager = isManager });
                }

                // 如果驗證失敗，返回原始頁面
                ViewData["isManager"] = isManager;
                ViewData["isLogin"] = isLogin;

                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return View("FullTime_Resignation", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤發生位置: {ex.StackTrace}");
                Console.WriteLine($"錯誤訊息: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"內部錯誤: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", $"離職申請失敗。錯誤訊息：{ex.Message}");

                var originalOrder = await _db.FullTime_order
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.FullTime_orderNo == originalOrderId);

                var viewModel = new FullTimeOrderViewModel
                {
                    FullTimeOrder = originalOrder,
                    FullTimeOrderDetails = originalOrder?.OrderDetails?.ToList() ??
                                         new List<FullTime_order_detail>()
                };

                return View("FullTime_Resignation", viewModel);
            }
        }
    }
    
}

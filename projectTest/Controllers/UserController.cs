using Microsoft.AspNetCore.Mvc;
using projectTest.Models;
using projectTest.ViewModels;
using X.PagedList;


namespace projectTest.Controllers
{
    public class UserController : Controller
    {
        private readonly dbContext _db; // 全域宣告資料庫物件

        // 在建構子中注入 IWebHostEnvironment
        public UserController(dbContext context)
        {
            _db = context ?? throw new ArgumentNullException(nameof(context));            
        }

        public IActionResult UserIndex(int? isLogin, int? isManager, int? page, bool? newHire, bool? renewal, bool? resignation, bool? salaryChange, bool? fullTimeAssistant, bool? partTimeAssistant, bool? temporaryWorker,
                bool? researchAssistant, bool? teachAssistant, bool? serviceAssistant, string projectName, string employedPerson, DateTime? searchDate)
        {

            int pageSize = 10; // 每頁顯示的申請單數量
            int pageNumber = page ?? 1; // 如果 page 為 null，則設置為第 1 頁

            var fullTimeOrdersQuery = _db.FullTime_order
                .Select(o => new FullTimeOrderViewModel
                {
                    FullTimeOrder = o,
                    FullTimeOrderDetails = _db.FullTime_order_detail
                                             .Where(d => d.FullTime_orderNo == o.FullTime_orderNo)
                                             .ToList()
                });


            var partTimeOrdersQuery = _db.PartTime_order.AsQueryable();


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
                fullTimeOrdersQuery = fullTimeOrdersQuery.Where(o => o.FullTimeOrderDetails.Any(detail => detail.PlanName.Contains(projectName)));
                partTimeOrdersQuery = partTimeOrdersQuery.Where(o => o.PlanName.Contains(projectName));
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
            ViewData["EmployedPerson"] = employedPerson;
            ViewData["SearchDate"] = searchDate;

            return View(pagedOrders);
        }



        public IActionResult OrderDetail(int? isLogin, int? isManager, string? order_id, string? category)
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

                return View("FullTime_detail", fullTimeOrderViewModel);
            }
            else
            {
                PartTime_order pt_orders = _db.PartTime_order.Find(order_id);

                if (pt_orders == null)
                {
                    return NotFound();
                }

                return View("PartTime_detail", pt_orders);
            }
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

                return RedirectToAction("UserIndex");
            }
            else
            {
                var pt_order = _db.PartTime_order.Find(order_id);
                if (pt_order == null) return NotFound();

                pt_order.Status = "已撤銷";
                _db.SaveChanges();

                return RedirectToAction("UserIndex");
            }
        }

        
    }
}

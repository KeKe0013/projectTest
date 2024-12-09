using Microsoft.AspNetCore.Mvc;
//using projectTest.Migrations;
using projectTest.Models;
using projectTest.Services;
using projectTest.ViewModels;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace projectTest.Controllers
{
    public class NewHireController : Controller
    {
        private readonly dbContext _db; // 全域宣告資料庫物件
        private readonly IFundingService _fundingService; // 全域宣告 FundingService 物件

        
        public NewHireController(dbContext context, IFundingService fundingService)
        {
            _db = context ?? throw new ArgumentNullException(nameof(context)); 
            _fundingService = fundingService ?? throw new ArgumentNullException(nameof(fundingService));
        }

        //GET，負責讀取
        public IActionResult partTimeAssistant(int? isLogin, int? isManager, string category)
        {
            ViewData["isManager"] = isManager;
            ViewData["isLogin"] = isLogin;

            var model = new Models.PartTime_order
            {
                Application = "新聘",
                Category = category,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult partTimeAssistant(Models.PartTime_order partTime_order, int? isLogin, int? isManager, string submitAction,string category)
        {

            ModelState.Remove(nameof(partTime_order.ApplicantName));
            ModelState.Remove(nameof(partTime_order.ApplicationOffice));
            ModelState.Remove(nameof(partTime_order.LeaderName));
            ModelState.Remove(nameof(partTime_order.Status));
            ModelState.Remove(nameof(partTime_order.StudyStatusMemo));
            ModelState.Remove(nameof(partTime_order.PartTime_orderNo));
            ModelState.Remove(nameof(partTime_order.PlanStartDate));
            ModelState.Remove(nameof(partTime_order.PlanEndDate));
            ModelState.Remove(nameof(partTime_order.PlanNo));
            ModelState.Remove(nameof(partTime_order.ResignDate));
            ModelState.Remove(nameof(partTime_order.SalaryChangeDate));


            int taiwanYear = DateTime.Now.Year - 1911;
            string type = category.StartsWith("勞僱型") ? "01" : category.StartsWith("學習型") ? "02" : "00";
            if (type == "00")
            {
                ModelState.AddModelError("", "無法辨識的類別。請確認類別是否正確。");
                return View(partTime_order);
            }
            int maxSerial = _db.FullTime_order
                .Where(o => o.FullTime_orderNo.ToString().StartsWith(taiwanYear.ToString() + type))
                .AsEnumerable()
                .Select(o => int.Parse(o.FullTime_orderNo.ToString().Substring(5)))
                .DefaultIfEmpty(0)
                .Max();

            maxSerial = Math.Max(maxSerial, _db.PartTime_order
                .Where(o => o.PartTime_orderNo.ToString().StartsWith(taiwanYear.ToString() + type))
                .AsEnumerable()
                .Select(o => int.Parse(o.PartTime_orderNo.ToString().Substring(5)))
                .DefaultIfEmpty(0)
                .Max());


            string newOrderID = $"{taiwanYear:D3}{type}{(maxSerial + 1):D5}";
            


            

            if (submitAction == "save")
            {
                partTime_order.Status = "未送出";
            }
            else if (submitAction == "submit")
            {
                partTime_order.Status = "待審核";
            }
            else
            {
                return RedirectToAction("UserIndex", "User");
            }

            string planStartDate = partTime_order.PlanStartDate;
            string planEndDate = partTime_order.PlanEndDate;
            string employedStartDate = partTime_order.EmployedStartDate;
            string employedEndDate = partTime_order.EmployedEndDate;
            
            if (!(string.Compare(planStartDate, employedStartDate) == -1 && string.Compare(employedEndDate, planEndDate) == -1 ))
            {
                ModelState.AddModelError("EmployedEndDate", "約用時間不在計畫時間範圍內");
            }
            else if(!(string.Compare(employedStartDate, employedEndDate) == -1))
            {
                ModelState.AddModelError("EmployedEndDate", "約用開始日期晚於結束日期");
            }
            if (partTime_order.StudyStatus != "本校學生" && partTime_order.StudyStatusMemo == null)
            {
                ModelState.AddModelError("studyStatus", "請輸入身分別資訊");
            }


            if (ModelState.IsValid)
            {
                partTime_order.PartTime_orderNo = newOrderID;

                partTime_order.Application = "新聘";
                partTime_order.Category = category;
                partTime_order.ApplicantName = "張佳琪";
                partTime_order.ApplicationOffice = "應用系統組";
                partTime_order.ApplicationTime = DateTime.Now;


                _db.PartTime_order.Add(partTime_order);
                _db.SaveChanges();
                return RedirectToAction("UserIndex", "User", new { isLogin = isLogin, isManager = isManager });
            }


           
            ViewData["isManager"] = isManager;
            ViewData["isLogin"] = isLogin;

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(partTime_order);
        }


        public IActionResult fullTimeAssistant(int? isLogin, int? isManager)
        {
            ViewData["isManager"] = isManager;
            ViewData["isLogin"] = isLogin;

            var fullTimeOrder = new Models.FullTime_order
            {
                Application = "新聘",
                Category = "勞僱型-專任助理"
            };

            
            var viewModel = new FullTimeOrderViewModel
            {
                FullTimeOrder = fullTimeOrder
                
            };

            return View(viewModel);
        }


        //POST，負責寫入
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult fullTimeAssistant(FullTimeOrderViewModel fullTime_order, int? isLogin, int? isManager, string submitAction)
        {
            ModelState.Clear();

            int taiwanYear = DateTime.Now.Year - 1911;
            string type = "01";
            
            int maxSerial = _db.FullTime_order
                .Where(o => o.FullTime_orderNo.ToString().StartsWith(taiwanYear.ToString() + type))
                .AsEnumerable()
                .Select(o => int.Parse(o.FullTime_orderNo.ToString().Substring(5)))
                .DefaultIfEmpty(0)
                .Max();

            maxSerial = Math.Max(maxSerial, _db.PartTime_order
                .Where(o => o.PartTime_orderNo.ToString().StartsWith(taiwanYear.ToString() + type))
                .AsEnumerable()
                .Select(o => int.Parse(o.PartTime_orderNo.ToString().Substring(5)))
                .DefaultIfEmpty(0)
                .Max());


            string newOrderID = $"{taiwanYear:D3}{type}{(maxSerial + 1):D5}";


            string planStartDate = fullTime_order.FullTimeOrderDetails.Max(d => d.PlanStartDate); // 最晚開始時間
            string planEndDate = fullTime_order.FullTimeOrderDetails.Min(d => d.PlanEndDate);   // 最早結束時間
            string employedStartDate = fullTime_order.FullTimeOrder.EmployedStartDate;
            string employedEndDate = fullTime_order.FullTimeOrder.EmployedEndDate;
            if (!(string.Compare(employedStartDate, employedEndDate) == -1))
            {
                ModelState.AddModelError("FullTimeOrder.EmployedEndDate", "聘僱開始日期晚於結束日期");
            }else if (!(string.Compare(planStartDate, employedStartDate) <= 0 && string.Compare(employedEndDate, planEndDate) <= 0))
            {
                ModelState.AddModelError("FullTimeOrder.EmployedEndDate", $"聘僱時間需在計畫時間範圍內({planStartDate} ~ {planEndDate})");
            }


            if (ModelState.IsValid)
            {
                try
                {
                    fullTime_order.FullTimeOrder.FullTime_orderNo = newOrderID;

                    fullTime_order.FullTimeOrder.ApplicantName = "張佳琪";
                    fullTime_order.FullTimeOrder.ApplicationOffice = "應用系統組";
                    fullTime_order.FullTimeOrder.ApplicationTime = DateTime.Now;
                    fullTime_order.FullTimeOrder.Category = "勞僱型-專任助理";
                    fullTime_order.FullTimeOrder.Status = submitAction == "save" ? "未送出" : "待審核";

                    _db.FullTime_order.Add(fullTime_order.FullTimeOrder);
                    _db.SaveChanges();

                    var orderNo = fullTime_order.FullTimeOrder.FullTime_orderNo;

                    if (fullTime_order.FullTimeOrderDetails != null && fullTime_order.FullTimeOrderDetails.Any())
                    {
                        foreach (var detail in fullTime_order.FullTimeOrderDetails)
                        {
                            detail.FullTime_orderNo = orderNo;

                            _db.FullTime_order_detail.Add(detail);
                        }

                        _db.SaveChanges();
                    }


                    return RedirectToAction("UserIndex", "User", new { isLogin, isManager });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "儲存資料時發生錯誤: " + ex.Message);
                    Console.WriteLine(ex.ToString());
                }
            }

            foreach (var modelState in ModelState)
            {
                var key = modelState.Key;
                var errors = modelState.Value.Errors;
                if (errors.Count > 0)
                {
                    Console.WriteLine($"Errors for '{key}':");
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
            }

            ViewData["isManager"] = isManager;
            ViewData["isLogin"] = isLogin;
            return View(fullTime_order);
        }

        [HttpGet]
        public JsonResult GetPlansByFundingSource(string fundingSource)
        {
            var plans = _fundingService.GetPlansByFundingSource(fundingSource);
            return Json(plans);
        }

        [HttpGet]
        public JsonResult GetPlanDetail(string planName,string fundingSource)
        {
            var PlanDetails = _fundingService.GetPlanDetail(planName, fundingSource);
            return Json(PlanDetails);
        }

        [HttpGet]
        public JsonResult GetStudentInfo(string studentid)
        {
            var StudentInfo = _fundingService.GetStudentInfo(studentid);
            return Json(StudentInfo);
        }

        [HttpGet]
        public IActionResult RenderAddPlanView(int index)
        {
            ViewData["Index"] = index;
            return PartialView("_AddPlan");
        }
    }
}

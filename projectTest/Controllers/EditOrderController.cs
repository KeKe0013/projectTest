using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using projectTest.Models;
using projectTest.ViewModels;

namespace projectTest.Controllers
{
    public class EditOrderController : Controller
    {
        private readonly dbContext _db;//先在全域宣告資料庫物件

        public EditOrderController(dbContext context) => _db = context ?? throw new ArgumentNullException(nameof(context));
        public IActionResult PartTime_Edit_NewHire(int? isLogin, int? isManager, string? order_id, string? category)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            var refererUrl = Request.Headers["Referer"].ToString();
            TempData["PreviousPage"] = refererUrl;

            PartTime_order pt_orders = _db.PartTime_order.Find(order_id);

            if (pt_orders == null)
            {
                return NotFound();
            }

            return View("PartTime_Edit_NewHire", pt_orders);
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PartTime_Edit_NewHire(PartTime_order partTime_order, int? isLogin, int? isManager, string submitAction)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            ModelState.Remove(nameof(partTime_order.ApplicationOffice));
            ModelState.Remove(nameof(partTime_order.Status));
            ModelState.Remove(nameof(partTime_order.PlanNo));
            ModelState.Remove(nameof(partTime_order.LeaderName));
            ModelState.Remove(nameof(partTime_order.ResignDate));
            ModelState.Remove(nameof(partTime_order.ApplicantName));
            ModelState.Remove(nameof(partTime_order.StudyStatusMemo));
            ModelState.Remove(nameof(partTime_order.SalaryChangeDate));

            var existingOrder = _db.PartTime_order.Find(partTime_order.PartTime_orderNo);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // 設定只有被修改的屬性
            existingOrder.StudyStatus = partTime_order.StudyStatus;
            existingOrder.StudyStatusMemo = partTime_order.StudyStatusMemo;
            existingOrder.EmployedName = partTime_order.EmployedName;
            existingOrder.phone = partTime_order.phone;
            existingOrder.ID = partTime_order.ID;
            existingOrder.Birthday = partTime_order.Birthday;
            existingOrder.EmployedStartDate = partTime_order.EmployedStartDate;
            existingOrder.EmployedEndDate = partTime_order.EmployedEndDate;
            existingOrder.EducationLevel = partTime_order.EducationLevel;
            existingOrder.SalaryCategory = partTime_order.SalaryCategory;
            existingOrder.Salary = partTime_order.Salary;
            existingOrder.IsTechPerson = partTime_order.IsTechPerson;
            existingOrder.ApplicationTime = DateTime.Now;
            
            if (submitAction == "cancel")
            {
                return RedirectToAction("ManagerIndex", "Manager");
            }

            _db.Entry(existingOrder).Property(x => x.ApplicationTime).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.AccountNo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.CommissionName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.StudyStatus).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.StudyStatusMemo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.phone).IsModified = true; 
            _db.Entry(existingOrder).Property(x => x.ID).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Birthday).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EducationLevel).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.SalaryCategory).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Salary).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.IsTechPerson).IsModified = true;

            if (ModelState.IsValid)
            {
                _db.SaveChanges();

                return Redirect(TempData["PreviousPage"].ToString());

            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(partTime_order);
        }       


        public IActionResult PartTime_Edit_Renewal(int? isLogin, int? isManager, string? order_id, string? category)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            var refererUrl = Request.Headers["Referer"].ToString();
            TempData["PreviousPage"] = refererUrl;

            PartTime_order pt_orders = _db.PartTime_order.Find(order_id);

            if (pt_orders == null)
            {
                return NotFound();
            }

            return View("PartTime_Edit_Renewal", pt_orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PartTime_Edit_Renewal(PartTime_order partTime_order, int? isLogin, int? isManager,string submitAction)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            ModelState.Clear();

            var existingOrder = _db.PartTime_order.Find(partTime_order.PartTime_orderNo);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // 設定只有被修改的屬性
            existingOrder.EmployedEndDate = partTime_order.EmployedEndDate;
            existingOrder.ApplicationTime = DateTime.Now;


            _db.Entry(existingOrder).Property(x => x.ApplicationTime).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.AccountNo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanNo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.CommissionName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.StudyStatus).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.StudyStatusMemo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.phone).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.ID).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Birthday).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EducationLevel).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.SalaryCategory).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Salary).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.IsTechPerson).IsModified = true;

            if (submitAction == "cancel")
            {
                return RedirectToAction("ManagerIndex", "Manager");
            }

            if (ModelState.IsValid)
            {
                _db.SaveChanges();

                return RedirectToAction("ManagerIndex", "Manager");
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(partTime_order);
        }


        public IActionResult PartTime_Edit_Resignation(int? isLogin, int? isManager, string? order_id, string? category)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            var refererUrl = Request.Headers["Referer"].ToString();
            TempData["PreviousPage"] = refererUrl;

            PartTime_order pt_orders = _db.PartTime_order.Find(order_id);

            if (pt_orders == null)
            {
                return NotFound();
            }

            return View("PartTime_Edit_Resignation", pt_orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PartTime_Edit_Resignation(PartTime_order partTime_order, int? isLogin, int? isManager, string submitAction)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            ModelState.Clear();

            var existingOrder = _db.PartTime_order.Find(partTime_order.PartTime_orderNo);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // 設定只有被修改的屬性
            existingOrder.ResignDate = partTime_order.ResignDate;
            existingOrder.ApplicationTime = DateTime.Now;


            _db.Entry(existingOrder).Property(x => x.ApplicationTime).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.AccountNo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanNo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.CommissionName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.StudyStatus).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.StudyStatusMemo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.phone).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.ID).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Birthday).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EducationLevel).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.SalaryCategory).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Salary).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.IsTechPerson).IsModified = true;

            if (submitAction == "cancel")
            {
                return RedirectToAction("ManagerIndex", "Manager");
            }

            if (ModelState.IsValid)
            {
                _db.SaveChanges();

                return RedirectToAction("ManagerIndex", "Manager");
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(partTime_order);
        }


        public IActionResult PartTime_Edit_SalaryChange(int? isLogin, int? isManager, string? order_id, string? category)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            var refererUrl = Request.Headers["Referer"].ToString();
            TempData["PreviousPage"] = refererUrl;

            PartTime_order pt_orders = _db.PartTime_order.Find(order_id);

            if (pt_orders == null)
            {
                return NotFound();
            }

            return View("PartTime_Edit_SalaryChange", pt_orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PartTime_Edit_SalaryChange(PartTime_order partTime_order, int? isLogin, int? isManager, string submitAction)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            ModelState.Clear();

            var existingOrder = _db.PartTime_order.Find(partTime_order.PartTime_orderNo);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // 設定只有被修改的屬性
            existingOrder.SalaryChangeDate = partTime_order.SalaryChangeDate;
            existingOrder.Salary = partTime_order.Salary;
            existingOrder.SalaryCategory = partTime_order.SalaryCategory;
            existingOrder.ApplicationTime = DateTime.Now;


            _db.Entry(existingOrder).Property(x => x.ApplicationTime).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.AccountNo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanNo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.CommissionName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.PlanEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.StudyStatus).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.StudyStatusMemo).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.phone).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.ID).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Birthday).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EducationLevel).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.SalaryCategory).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Salary).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.IsTechPerson).IsModified = true;

            if (submitAction == "cancel")
            {
                return RedirectToAction("ManagerIndex", "Manager");
            }

            if (ModelState.IsValid)
            {
                _db.SaveChanges();

                return RedirectToAction("ManagerIndex", "Manager");
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(partTime_order);
        }


        public IActionResult FullTime_Edit_NewHire(int? isLogin, int? isManager, string? order_id)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            var refererUrl = Request.Headers["Referer"].ToString();
            TempData["PreviousPage"] = refererUrl;

            var fullTimeOrder = _db.FullTime_order.Find(order_id);

            if (fullTimeOrder == null)
            {
                return NotFound();
            }

            FullTimeOrderViewModel ft_orders = new FullTimeOrderViewModel
            {
                FullTimeOrder = _db.FullTime_order.Find(order_id),
                FullTimeOrderDetails = _db.FullTime_order_detail
                                             .Where(d => d.FullTime_orderNo == order_id)
                                             .ToList()
            };

            // 返回視圖並傳遞 ViewModel
            return View("FullTime_Edit_NewHire", ft_orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FullTime_Edit_NewHire(FullTimeOrderViewModel fullTime_order, int? isLogin, int? isManager, string submitAction)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            ModelState.Clear();

            var existingOrder = _db.FullTime_order.Find(fullTime_order.FullTimeOrder.FullTime_orderNo);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // 設定只有被修改的屬性
            existingOrder.EmployedName = fullTime_order.FullTimeOrder.EmployedName;
            existingOrder.Phone = fullTime_order.FullTimeOrder.Phone;
            existingOrder.ID = fullTime_order.FullTimeOrder.ID;
            existingOrder.Birthday = fullTime_order.FullTimeOrder.Birthday;
            existingOrder.EmployedStartDate = fullTime_order.FullTimeOrder.EmployedStartDate;
            existingOrder.EmployedEndDate = fullTime_order.FullTimeOrder.EmployedEndDate;
            existingOrder.JobTitle = fullTime_order.FullTimeOrder.JobTitle;
            existingOrder.EducationLevel = fullTime_order.FullTimeOrder.EducationLevel;
            existingOrder.Salary = fullTime_order.FullTimeOrder.Salary;
            existingOrder.SalaryGrade = fullTime_order.FullTimeOrder.SalaryGrade;
            existingOrder.ApplicationTime = DateTime.Now;
            if (submitAction == "cancel")
            {
                return RedirectToAction("ManagerIndex", "Manager");
            }

            _db.Entry(existingOrder).Property(x => x.ApplicationTime).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Phone).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.ID).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Birthday).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EducationLevel).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.SalaryGrade).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Salary).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.JobTitle).IsModified = true;

            if (ModelState.IsValid)
            {
                _db.SaveChanges();

                return Redirect(TempData["PreviousPage"].ToString());

            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(fullTime_order);
        }


        public IActionResult FullTime_Edit_Renewal(int? isLogin, int? isManager, string? order_id)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            var refererUrl = Request.Headers["Referer"].ToString();
            TempData["PreviousPage"] = refererUrl;

            var fullTimeOrder = _db.FullTime_order.Find(order_id);

            if (fullTimeOrder == null)
            {
                return NotFound();
            }

            FullTimeOrderViewModel ft_orders = new FullTimeOrderViewModel
            {
                FullTimeOrder = _db.FullTime_order.Find(order_id),
                FullTimeOrderDetails = _db.FullTime_order_detail
                                             .Where(d => d.FullTime_orderNo == order_id)
                                             .ToList()
            };

            // 返回視圖並傳遞 ViewModel
            return View("FullTime_Edit_Renewal", ft_orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FullTime_Edit_Renewal(FullTimeOrderViewModel fullTime_order, int? isLogin, int? isManager, string submitAction)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            ModelState.Clear();

            var existingOrder = _db.FullTime_order.Find(fullTime_order.FullTimeOrder.FullTime_orderNo);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // 設定只有被修改的屬性
            existingOrder.EmployedEndDate = fullTime_order.FullTimeOrder.EmployedEndDate;
            existingOrder.ApplicationTime = DateTime.Now;


            if (submitAction == "cancel")
            {
                return RedirectToAction("ManagerIndex", "Manager");
            }
                       

            _db.Entry(existingOrder).Property(x => x.ApplicationTime).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Phone).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.ID).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Birthday).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EducationLevel).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.SalaryGrade).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Salary).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.JobTitle).IsModified = true;

            if (ModelState.IsValid)
            {
                _db.SaveChanges();

                return Redirect(TempData["PreviousPage"].ToString());

            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(fullTime_order);
        }

        public IActionResult FullTime_Edit_Resignation(int? isLogin, int? isManager, string? order_id)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            var refererUrl = Request.Headers["Referer"].ToString();
            TempData["PreviousPage"] = refererUrl;

            var fullTimeOrder = _db.FullTime_order.Find(order_id);

            if (fullTimeOrder == null)
            {
                return NotFound();
            }

            FullTimeOrderViewModel ft_orders = new FullTimeOrderViewModel
            {
                FullTimeOrder = _db.FullTime_order.Find(order_id),
                FullTimeOrderDetails = _db.FullTime_order_detail
                                             .Where(d => d.FullTime_orderNo == order_id)
                                             .ToList()
            };

            // 返回視圖並傳遞 ViewModel
            return View("FullTime_Edit_Resignation", ft_orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FullTime_Edit_Resignation(FullTimeOrderViewModel fullTime_order, int? isLogin, int? isManager, string submitAction)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            ModelState.Clear();

            var existingOrder = _db.FullTime_order.Find(fullTime_order.FullTimeOrder.FullTime_orderNo);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // 設定只有被修改的屬性
            existingOrder.ResignDate = fullTime_order.FullTimeOrder.ResignDate;
            existingOrder.ApplicationTime = DateTime.Now;


            if (submitAction == "cancel")
            {
                return RedirectToAction("ManagerIndex", "Manager");
            }


            _db.Entry(existingOrder).Property(x => x.ApplicationTime).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Phone).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.ID).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Birthday).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EducationLevel).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.SalaryGrade).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Salary).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.JobTitle).IsModified = true;

            if (ModelState.IsValid)
            {
                _db.SaveChanges();

                return Redirect(TempData["PreviousPage"].ToString());

            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(fullTime_order);
        }

        public IActionResult FullTime_Edit_SalaryChange(int? isLogin, int? isManager, string? order_id)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            var refererUrl = Request.Headers["Referer"].ToString();
            TempData["PreviousPage"] = refererUrl;

            var fullTimeOrder = _db.FullTime_order.Find(order_id);

            if (fullTimeOrder == null)
            {
                return NotFound();
            }

            FullTimeOrderViewModel ft_orders = new FullTimeOrderViewModel
            {
                FullTimeOrder = _db.FullTime_order.Find(order_id),
                FullTimeOrderDetails = _db.FullTime_order_detail
                                             .Where(d => d.FullTime_orderNo == order_id)
                                             .ToList()
            };

            // 返回視圖並傳遞 ViewModel
            return View("FullTime_Edit_SalaryChange", ft_orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FullTime_Edit_SalaryChange(FullTimeOrderViewModel fullTime_order, int? isLogin, int? isManager, string submitAction)
        {
            ViewData["isLogin"] = isLogin ?? 0;
            ViewData["isManager"] = isManager ?? 0;

            ModelState.Clear();

            var existingOrder = _db.FullTime_order.Find(fullTime_order.FullTimeOrder.FullTime_orderNo);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // 設定只有被修改的屬性
            existingOrder.SalaryChangeDate = fullTime_order.FullTimeOrder.SalaryChangeDate;
            existingOrder.Salary = fullTime_order.FullTimeOrder.Salary;
            existingOrder.SalaryGrade = fullTime_order.FullTimeOrder.SalaryGrade;
            existingOrder.SalaryChangeReason = fullTime_order.FullTimeOrder.SalaryChangeReason;
            existingOrder.ApplicationTime = DateTime.Now;


            if (submitAction == "cancel")
            {
                return RedirectToAction("ManagerIndex", "Manager");
            }


            _db.Entry(existingOrder).Property(x => x.ApplicationTime).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedName).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Phone).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.ID).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Birthday).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedStartDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EmployedEndDate).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.EducationLevel).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.SalaryGrade).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.Salary).IsModified = true;
            _db.Entry(existingOrder).Property(x => x.JobTitle).IsModified = true;

            if (ModelState.IsValid)
            {
                _db.SaveChanges();

                return Redirect(TempData["PreviousPage"].ToString());

            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(fullTime_order);
        }
    }
}

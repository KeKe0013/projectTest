using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using projectTest.Attributes;

namespace projectTest.Models;

public partial class FullTime_order
{
    
    public string FullTime_orderNo { get; set; }
    public string ApplicantName { get; set; }
    public string ApplicationOffice { get; set; }
    public DateTime ApplicationTime { get; set; }
    public string Application { get; set; }
    public string Category { get; set; }
    [Required(ErrorMessage = "請輸入聘僱人員姓名")]
    public string EmployedName { get; set; }
    [Required(ErrorMessage = "請輸入身分證")]
    public string ID { get; set; }
    [Required(ErrorMessage = "請輸入出生日期")]
    public string Birthday { get; set; }
    [Required(ErrorMessage = "請選擇聘僱開始日期")]
    public string EmployedStartDate { get; set; }
    [Required(ErrorMessage = "請選擇聘僱結束日期")]
    public string EmployedEndDate { get; set; }
    [Required(ErrorMessage = "請輸入聯絡電話")]
    public string Phone { get; set; }
    [Required(ErrorMessage = "請選擇學歷")]
    public string EducationLevel { get; set; }
    [Required(ErrorMessage = "請輸入職稱")]
    public string JobTitle { get; set; }
    public string SalaryGrade { get; set; }
    [Required(ErrorMessage = "請輸入薪水金額")]
    public string Salary { get; set; }
    public string SalaryChangeReason { get; set; }
    public string ResignDate { get; set; }
    public string SalaryChangeDate { get; set; }
    public string Status { get; set; }
    public string isApplyRenewal { get; set; }
    public string isApplySalaryChange { get; set; }
    public string isApplyResignation { get; set; }

    public ICollection<FullTime_order_detail> OrderDetails { get; set; }
}
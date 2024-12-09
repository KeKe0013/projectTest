using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using projectTest.Attributes;

namespace projectTest.Models;

public partial class PartTime_order
{

    public string PartTime_orderNo { get; set; }
    public string ApplicantName { get; set; }
    public string ApplicationOffice { get; set; }
    public DateTime ApplicationTime { get; set; }
    public string Application { get; set; }
    [Required(ErrorMessage = "請選擇類別")]
    public string Category { get; set; }
    
    [Required(ErrorMessage = "請選擇計畫名稱")]
    public string PlanName { get; set; }
    [Required(ErrorMessage = "請輸入會計編號")]
    public string AccountNo { get; set; }
    public string PlanNo { get; set; }
    public string CommissionName { get; set; }
    public string PlanStartDate { get; set; }
    public string PlanEndDate { get; set; }
    public string LeaderName { get; set; }
    [Required(ErrorMessage = "請輸入約用人員姓名")]
    public string EmployedName { get; set; }
    [Required(ErrorMessage = "請輸入身分證字號")]
    public string ID { get; set; }
    [Required(ErrorMessage = "請選擇出生日期")]
    public string Birthday { get; set; }
    [Required(ErrorMessage = "請選擇開始時間")]
    public string EmployedStartDate { get; set; }
    [Required(ErrorMessage = "請選擇結束時間")]
    public string EmployedEndDate { get; set; }
    [Required(ErrorMessage = "請輸入手機")]
    public string phone { get; set; }
    [Required(ErrorMessage = "請選擇身分別")]
    public string StudyStatus { get; set; }
    public string StudyStatusMemo { get; set; }
    [Required(ErrorMessage = "請選擇級別")]
    public string EducationLevel { get; set; }
    [Required(ErrorMessage = "請選擇薪水類型")]
    public string SalaryCategory { get; set; }
    [Required(ErrorMessage = "請輸入薪水金額")]
    public string Salary { get; set; }
    [Required(ErrorMessage = "請選擇是否為首次參與國科會研究人員")]
    public string IsTechPerson { get; set; }
    public string ResignDate { get; set; }
    public string SalaryChangeDate { get; set; }
    public string Status { get; set; }
    public string isApplyRenewal { get; set; }
    public string isApplySalaryChange { get; set; }
    public string isApplyResignation { get; set; }
    public string projtype { get; set; }
}
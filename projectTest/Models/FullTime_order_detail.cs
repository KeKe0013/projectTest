using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using projectTest.Attributes;

namespace projectTest.Models
{
    public class FullTime_order_detail
    {
        public string FullTime_orderNo { get; set; }
        [NotNone]
        [Required(ErrorMessage = "請選擇計畫名稱")]
        public string PlanName { get; set; }
        [Required(ErrorMessage = "請輸入會計編號")]
        public string AccountNo { get; set; }
        public string PlanNo { get; set; }
        public string CommissionName { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
        public string LeaderName { get; set; }
        public string projtype { get; set; }


        [ForeignKey("FullTime_orderNo")]
        public FullTime_order FullTimeOrder { get; set; }
    }
}

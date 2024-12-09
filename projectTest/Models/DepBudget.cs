namespace projectTest.Models
{
    public class DepBudget
    {
        public string depno { get; set; }
        public string depid { get; set; }
        public string projectno { get; set; }
        public string fundno { get; set; }
        public string fundname { get; set; }
        public decimal budget { get; set; }
        public decimal budget_remain { get; set; }
        public string sourcetype { get; set; }
        public string leader { get; set; }
        public string leaderid { get; set; }
        public string depcname { get; set; }
    }
}

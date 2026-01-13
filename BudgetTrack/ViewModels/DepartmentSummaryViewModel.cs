namespace BudgetTrack.ViewModels
{
    public class DepartmentSummaryViewModel
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal AnnualBudget { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Remaining { get; set; }
        public double SpendingPercentage { get; set; }
        public string PeriodType { get; set; } = "Quarterly";
    }
}
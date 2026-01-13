namespace BudgetTrack.ViewModels
{
    public class DepartmentPredictionViewModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public float PredictedBudget { get; set; }
    }
}
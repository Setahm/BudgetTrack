using System.Collections.Generic;

namespace BudgetTrack.ViewModels
{
    public class CompanyDetailsViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string PeriodType { get; set; } = "Quarterly";
        public int SelectedYear { get; set; }

        public List<DepartmentSummaryViewModel> Departments { get; set; } = new();
    }
}
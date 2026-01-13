using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTrack.Models
{
    public class Department
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "اسم القسم مطلوب")]
        public string Name { get; set; }
        [Required(ErrorMessage = "الميزانية مطلوبة")]
        public decimal AnnualBudget { get; set; }
        [Required(ErrorMessage = "السنة مطلوبة")]
        public int Year { get; set; }
        public string? PeriodType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
        public List<Expense> Expenses { get; set; } = new();
        public List<Prediction>? Predictions { get; set; }
        [NotMapped]
        public decimal TotalExpenses => Expenses?.Sum(e => e.Amount) ?? 0;
        [NotMapped]
        public decimal Remaining => AnnualBudget - TotalExpenses;
        [NotMapped]
        public decimal SpendingPercentage =>
            AnnualBudget == 0 ? 0 : Math.Round((TotalExpenses / AnnualBudget) * 100, 2);
    }
}
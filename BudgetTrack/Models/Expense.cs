using System;

namespace BudgetTrack.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Category { get; set; }  
        public Department? Department { get; set; }
    }
}
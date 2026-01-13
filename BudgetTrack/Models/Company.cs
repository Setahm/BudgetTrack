using System;
using System.Collections.Generic;

namespace BudgetTrack.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PeriodType { get; set; }
        public List<Department> Departments { get; set; } = new();
        public List<User>? Users { get; set; }
    }
}
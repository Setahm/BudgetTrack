using System;
using System.Collections.Generic;

namespace BudgetTrack.Models;

public partial class Prediction
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public int Year { get; set; }
    public decimal PredictedAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string DepartmentName { get; set; } = string.Empty; 
    public virtual Department Department { get; set; } = null!;
}
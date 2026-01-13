using System;
using System.Collections.Generic;

namespace BudgetTrack.Models;

public partial class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;
    public int? CompanyId { get; set; }
    public bool IsApproved { get; set; }   
    public DateTime CreatedAt { get; set; }
    public virtual Company? Company { get; set; }
}
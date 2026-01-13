using BudgetTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly BudgetTrackDbContext _context;

    public AccountController(BudgetTrackDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Profile()
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login", "Auth");

        var user = _context.Users
            .Include(u => u.Company)
            .FirstOrDefault(u => u.Email == email);

        if (user == null)
            return RedirectToAction("Login", "Auth");

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(string currentPassword, string newPassword)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login", "Auth");

        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
            return RedirectToAction("Login", "Auth");

        if (user.PasswordHash != currentPassword)
        {
            TempData["Error"] = "كلمة المرور الحالية غير صحيحة.";
            return RedirectToAction("Profile");
        }

        user.PasswordHash = newPassword;
        _context.SaveChanges();

        TempData["Success"] = "تم تغيير كلمة المرور بنجاح.";
        return RedirectToAction("Profile");
    }
}
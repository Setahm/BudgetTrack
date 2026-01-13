using BudgetTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

public class RegisterController : Controller
{
    private readonly BudgetTrackDbContext _context;

    public RegisterController(BudgetTrackDbContext context)
    {
        _context = context;
    }

    // GET: صفحة التسجيل
    public IActionResult CompanyAdmin()
    {
        ViewBag.Companies = new SelectList(_context.Companies, "Id", "Name");
        return View();
    }

    // POST: حفظ البيانات
    [HttpPost]
    public IActionResult CompanyAdmin([Bind("Name,Email,PasswordHash,CompanyId")] User user)
    {
        // نعبّي الخصائص قبل التحقق
        user.Role = "CompanyAdmin";
        user.IsApproved = false;
        user.CreatedAt = DateTime.Now;

        // نحذف التحقق عن Role لأنه ما يجي من النموذج
        ModelState.Remove("Role");

        // نطبع CompanyId للتأكد
        Console.WriteLine($"CompanyId = {user.CompanyId}");

        // نتحقق من النموذج ونطبع الأخطاء لو فيه
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            return Content("ModelState = FALSE | " + string.Join(" | ", errors.Select(e => e.ErrorMessage)));
        }

        // نحفظ المستخدم
        _context.Users.Add(user);
        _context.SaveChanges();

        // نرجع لصفحة تسجيل الدخول
        return RedirectToAction("Login", "Auth");
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BudgetTrack.Models;
using System.Linq;

namespace BudgetTrack.Controllers
{
    public class AuthController : Controller
    {
        private readonly BudgetTrackDbContext _context;

        public AuthController(BudgetTrackDbContext context)
        {
            _context = context;
        }

       
        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            var activeCompanies = _context.Companies
                .Where(c => c.IsActive == true)
                .ToList();

            ViewBag.Companies = new SelectList(activeCompanies, "Id", "Name");

            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            // تعبئة الخصائص المطلوبة قبل ModelState
            user.Role = "CompanyAdmin";
            user.IsApproved = false;
            user.CreatedAt = DateTime.Now;

            // إزالة الخصائص اللي ماتجي من الفورم
            ModelState.Remove("Company");
            ModelState.Remove("Role");

            // جمع الأخطاء لعرضها في الصفحة
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            TempData["ModelErrors"] = string.Join(" | ", errors);

            if (!ModelState.IsValid)
            {
                var activeCompanies = _context.Companies
                    .Where(c => c.IsActive == true)
                    .ToList();

                ViewBag.Companies = new SelectList(activeCompanies, "Id", "Name");

                return View(user);
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "تم إرسال طلب التسجيل، بانتظار موافقة الأدمن.";
            return RedirectToAction("Login");
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user == null)
            {
                TempData["Error"] = "البريد أو كلمة المرور غير صحيحة";
                return RedirectToAction("Login");
            }

            if (user.Role == "CompanyAdmin" && user.IsApproved == false)
            {
                TempData["Error"] = "حسابك بانتظار موافقة الأدمن.";
                return RedirectToAction("Login");
            }

            // أهم شيء — حفظ بيانات المستخدم في الجلسة
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserEmail", user.Email);   
            HttpContext.Session.SetString("UserName", user.Name);     

            if (user.CompanyId.HasValue)
            {
                HttpContext.Session.SetString("CompanyId", user.CompanyId.Value.ToString());
            }

            if (user.Role == "Admin")
                return RedirectToAction("Index", "AdminDashboard");

            return RedirectToAction("Index", "CompanyDashboard");
        }
       
        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home"); 
        }
    }
}
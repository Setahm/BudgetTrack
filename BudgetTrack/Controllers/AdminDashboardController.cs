using BudgetTrack.Filters;
using BudgetTrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class AdminDashboardController : Controller
{
    private readonly BudgetTrackDbContext _context;

    public AdminDashboardController(BudgetTrackDbContext context)
    {
        _context = context;
    }

    // ================================
    //  Dashboard + Pending Count
    // ================================
    [AuthFilter]
    [RoleFilter("Admin")]
    public IActionResult Index()
    {
        ViewBag.PendingUsersCount = _context.Users
            .Count(u => u.Role == "CompanyAdmin" && !u.IsApproved);

        var companies = _context.Companies
            .Include(c => c.Departments)
            .Include(c => c.Users)
            .ToList();

        return View(companies);
    }

    // ================================
    //  Pending Users Page
    // ================================
    [AuthFilter]
    [RoleFilter("Admin")]
    public IActionResult PendingUsers()
    {
        var users = _context.Users
            .Where(u => u.Role == "CompanyAdmin" && !u.IsApproved)
            .Include(u => u.Company)
            .ToList();

        return View(users);
    }

    // ================================
    //  Approve User
    // ================================
    [AuthFilter]
    [RoleFilter("Admin")]
    public IActionResult ApproveUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        user.IsApproved = true;
        _context.SaveChanges();

        return RedirectToAction("PendingUsers");
    }

    // ================================
    //  Reject User
    // ================================
    [AuthFilter]
    [RoleFilter("Admin")]
    public IActionResult RejectUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        _context.SaveChanges();

        return RedirectToAction("PendingUsers");
    }

    // ================================
    //  CRUD: Companies
    // ================================
    [AuthFilter]
    [RoleFilter("Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [AuthFilter]
    [RoleFilter("Admin")]
    public IActionResult Create(Company company)
    {
        if (ModelState.IsValid)
        {
            company.CreatedAt = DateTime.Now;
            _context.Companies.Add(company);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(company);
    }

    [AuthFilter]
    [RoleFilter("Admin")]
    public IActionResult Edit(int id)
    {
        var company = _context.Companies.Find(id);
        if (company == null)
            return NotFound();

        return View(company);
    }

    [HttpPost]
    [AuthFilter]
    [RoleFilter("Admin")]
    public IActionResult Edit(Company company)
    {
        if (ModelState.IsValid)
        {
            _context.Companies.Update(company);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(company);
    }

    [HttpPost]
    [AuthFilter]
    [RoleFilter("Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        var company = _context.Companies.Find(id);

        if (company == null)
            return NotFound();

        _context.Companies.Remove(company);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetTrack.Models;

namespace BudgetTrack.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly BudgetTrackDbContext _context;

        public ExpensesController(BudgetTrackDbContext context)
        {
            _context = context;
        }

        // عرض المصروفات لقسم معيّن
        public IActionResult Index(int departmentId)
        {
            var expenses = _context.Expenses
                .Where(e => e.DepartmentId == departmentId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToList();

            ViewBag.DepartmentId = departmentId;

            return View(expenses);
        }

        // صفحة إضافة مصروف
        public IActionResult Create(int departmentId)
        {
            ViewBag.DepartmentId = departmentId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Expense model)
        {
            _context.Expenses.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index", new { departmentId = model.DepartmentId });
        }

        // صفحة تعديل مصروف
        public IActionResult Edit(int id)
        {
            var expense = _context.Expenses.Find(id);
            return View(expense);
        }

        [HttpPost]
        public IActionResult Edit(Expense model)
        {
            _context.Expenses.Update(model);
            _context.SaveChanges();

            return RedirectToAction("Index", new { departmentId = model.DepartmentId });
        }

        // حذف مصروف
        public IActionResult Delete(int id)
        {
            var expense = _context.Expenses.Find(id);
            if (expense == null)
            {
                return NotFound();
            }
            int deptId = expense.DepartmentId;

            _context.Expenses.Remove(expense);
            _context.SaveChanges();

            return RedirectToAction("Index", new { departmentId = deptId });
        }
    }
}
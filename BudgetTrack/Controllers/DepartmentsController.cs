using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetTrack.Models;

namespace BudgetTrack.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly BudgetTrackDbContext _context;

        public DepartmentsController(BudgetTrackDbContext context)
        {
            _context = context;
        }

        // ============================
        // GET: Index
        // ============================
        public IActionResult Index()
        {
            var companyIdString = HttpContext.Session.GetString("CompanyId");

            if (string.IsNullOrEmpty(companyIdString))
                return Content("❌ CompanyId مفقود من الجلسة.");

            int companyId = int.Parse(companyIdString);

            var departments = _context.Departments
                .Where(d => d.CompanyId == companyId)
                .ToList();

            return View(departments);
        }

        // ============================
        // GET: Create
        // ============================
        public IActionResult Create()
        {
            var companyIdString = HttpContext.Session.GetString("CompanyId");

            if (string.IsNullOrEmpty(companyIdString))
                return Content("❌ CompanyId مفقود من الجلسة.");

            int companyId = int.Parse(companyIdString);

            return View(new Department
            {
                CompanyId = companyId
            });
        }

        // ============================
        // POST: Create
        // ============================
        [HttpPost]
        public IActionResult Create(Department model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.CompanyId == 0)
                return Content("❌ CompanyId = 0 | النموذج لم يستقبل الشركة.");

            try
            {
                model.CreatedAt = DateTime.Now;

                _context.Departments.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content("❌ خطأ أثناء الحفظ:\n" + ex.Message);
            }
        }

        // ============================
        // GET: Edit
        // ============================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments.FindAsync(id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // ============================
        // POST: Edit
        // ============================
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Department model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Departments.Any(e => e.Id == model.Id))
                    return NotFound();

                throw;
            }
        }

        // ============================
        // GET: Delete
        // ============================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
                return NotFound();

            return View(department);
        }

        // ============================
        // POST: DeleteConfirmed
        // ============================
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetTrack.Models;
using BudgetTrack.ViewModels;
using BudgetTrack.Services;

namespace BudgetTrack.Controllers
{
    public class CompanyDashboardController : Controller
    {
        private readonly BudgetTrackDbContext _context;
        private readonly PredictionService _predictionService;

        public CompanyDashboardController(BudgetTrackDbContext context, PredictionService predictionService)
        {
            _context = context;
            _predictionService = predictionService;
        }
        public IActionResult Index()
        {
            var companyIdString = HttpContext.Session.GetString("CompanyId");

            if (!int.TryParse(companyIdString, out int companyId))
                return Unauthorized();

            return RedirectToAction("CompanyHome", new { id = companyId });
        }
        public IActionResult CompanyHome(int id)
        {
            var company = _context.Companies
                .Include(c => c.Departments)
                .FirstOrDefault(c => c.Id == id);

            if (company == null)
                return NotFound();

            return View(company);
        }

        public IActionResult CompanyDetails(int id, int year = 2026)
        {
            var company = _context.Companies
                .Include(c => c.Departments)
                .ThenInclude(d => d.Expenses)
                .FirstOrDefault(c => c.Id == id);

            if (company == null)
                return NotFound();

            var departmentData = company.Departments
                .Select(d => new DepartmentSummaryViewModel
                {
                    DepartmentId = d.Id,
                    Name = d.Name ?? "بدون اسم",
                    AnnualBudget = d.AnnualBudget,
                    TotalExpenses = d.Expenses
                        .Where(e => e.ExpenseDate.Year == year)
                        .Sum(e => e.Amount),
                    Remaining = d.AnnualBudget -
                        d.Expenses
                        .Where(e => e.ExpenseDate.Year == year)
                        .Sum(e => e.Amount),
                    SpendingPercentage = d.AnnualBudget == 0
                        ? 0
                        : Math.Round(
                            (double)(
                                d.Expenses
                                .Where(e => e.ExpenseDate.Year == year)
                                .Sum(e => e.Amount) / d.AnnualBudget
                            ) * 100, 2),
                    PeriodType = d.PeriodType ?? "Quarterly"
                })
                .ToList();

            var viewModel = new CompanyDetailsViewModel
            {
                CompanyId = company.Id,
                CompanyName = company.Name ?? "شركة بدون اسم",
                PeriodType = company.PeriodType ?? "Quarterly",
                SelectedYear = year,
                Departments = departmentData
            };

            return View(viewModel);
        }
        public IActionResult Predictions(int id)
        {
            var company = _context.Companies
                .Include(c => c.Departments)
                .FirstOrDefault(c => c.Id == id);

            if (company == null)
                return NotFound();

            // تشغيل التنبؤ 
            var predictions = _predictionService.PredictNextYearBudgetPerDepartment(id);

            // تجهيز البيانات للعرض
            var model = company.Departments.Select(d => new DepartmentPredictionViewModel
            {
                DepartmentId = d.Id,
                DepartmentName = d.Name ?? "بدون اسم",
                PredictedBudget = predictions.ContainsKey(d.Id) ? predictions[d.Id] : 0
            }).ToList();

            // مجموع الشركة كامل
            ViewBag.TotalCompanyPrediction = model.Sum(m => m.PredictedBudget);

            return View(model);
        }
    }
}
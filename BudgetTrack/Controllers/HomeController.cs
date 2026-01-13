using Microsoft.AspNetCore.Mvc;
using BudgetTrack.Models;

namespace BudgetTrack.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new ViewModels.LoginViewModel());
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
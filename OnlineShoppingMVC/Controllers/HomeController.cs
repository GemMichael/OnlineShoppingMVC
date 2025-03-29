using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingMVC.Models;

namespace OnlineShoppingMVC.Controllers
{
    public class HomeController : Controller
    {
        // Logger for recording application events and errors
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // GET: Displays the home page
        public IActionResult Index()
        {
            return View();
        }

        // GET: Displays the privacy policy page
        public IActionResult Privacy()
        {
            return View();
        }
        // Handles application errors and displays an error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

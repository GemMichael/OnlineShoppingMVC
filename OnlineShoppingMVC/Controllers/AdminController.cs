using Microsoft.AspNetCore.Mvc;

namespace OnlineShoppingMVC.Controllers
{
    public class AdminController : Controller
    {
        // Hardcoded admin credentials (for demonstration purposes only - should use secure authentication)
        private const string AdminUsername = "admin";
        private const string AdminPassword = "password123";

        // GET: Displays the admin login page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Handles admin login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == AdminUsername && password == AdminPassword)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index", "Products");
            }
            ViewBag.Message = "Invalid credentials!";
            return View();
        }
        // Logs out the admin by clearing session data
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            return RedirectToAction("Index", "Home");
        }



    }
}

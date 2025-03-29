using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingMVC.Data;
using OnlineShoppingMVC.Models;
using System.Threading.Tasks;

namespace OnlineShoppingMVC.Controllers
{
    [Authorize] // Ensures only logged-in users can access this page
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Load Profile Page
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var model = new ProfileViewModel
            {
                Email = user.Email!,
                Address = user.Address ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty
            };

            return View(model);
        }

        // Update Profile Information
        [HttpPost]
        public async Task<IActionResult> Update(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            user.Email = model.Email;
            user.Address = model.Address;
            user.PhoneNumber = model.Phone;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                ViewBag.Message = "Profile updated successfully!";
            }
            else
            {
                ViewBag.Error = "Error updating profile.";
            }

            return View("Index", model);
        }
    }
}

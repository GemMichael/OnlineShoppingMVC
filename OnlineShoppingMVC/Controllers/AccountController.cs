using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingMVC.Models;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    // Constructor to initialize UserManager and SignInManager
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: Register page
    public IActionResult Register() => View();

    // POST: Handles user registration
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        Console.WriteLine(" Register method called");

        if (ModelState.IsValid)
        {
            Console.WriteLine("Model state is valid");
            // Create new user object from the submitted model data
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Address = model.Address,
                PhoneNumber = model.Phone
            };

            Console.WriteLine($"Creating user: {user.Email}");
            // Attempt to create the user with the provided password
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                Console.WriteLine("User created successfully");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("PublicIndex", "Products");  
            }
            else
            {
                Console.WriteLine(" User creation failed");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Code} - {error.Description}");
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        else
        {
            Console.WriteLine(" Model state is invalid");
        }

        return View(model);
    }

    // Login Action
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("PublicIndex", "Products");  
            }
            ModelState.AddModelError("", "Invalid login attempt.");
        }
        return View(model);
    }

    // Logout Action
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

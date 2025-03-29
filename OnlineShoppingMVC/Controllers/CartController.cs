using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingMVC.Data;
using OnlineShoppingMVC.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnlineShoppingMVC.Controllers
{
    [Authorize] // Ensure only logged-in users can access
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // View Cart
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                                         .Include(c => c.Product)
                                         .Where(c => c.UserId == user.Id)
                                         .ToListAsync();
            return View(cartItems);
        }

        //  Add to Cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (quantity <= 0) return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItem = await _context.CartItems
                                         .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem { UserId = user.Id, ProductId = productId, Quantity = quantity };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Checkout Page (GET)
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                                          .Include(c => c.Product)
                                          .Where(c => c.UserId == user.Id)
                                          .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var model = new CheckoutViewModel
            {
                TotalAmount = cartItems.Sum(item => item.Quantity * (item.Product?.Price ?? 0))
            };

            return View(model);
        }



        //  Checkout (POST)
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (user == null)
            {
                TempData["Error"] = "User not found. Please log in.";
                return RedirectToAction("Login", "Account");
            }

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c => (c.Product?.Price ?? 0) * c.Quantity), 
                Status = "Pending",
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Price = c.Product?.Price ?? 0, 
                    Quantity = c.Quantity
                }).ToList()
            };


            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems); 
            await _context.SaveChangesAsync(); 

            return RedirectToAction("OrderConfirmation");
        }




        // Checkout Selected Items (POST)
        [HttpPost]
        public async Task<IActionResult> CheckoutSelected(List<int> selectedItems)
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                TempData["Error"] = "No items selected for checkout.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var selectedCartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == user.Id && selectedItems.Contains(ci.Id))
                .ToListAsync();

            if (!selectedCartItems.Any())
            {
                TempData["Error"] = "Selected items not found.";
                return RedirectToAction("Index");
            }

            var totalAmount = selectedCartItems.Sum(ci => (ci.Product?.Price ?? 0) * ci.Quantity);

            var checkoutModel = new CheckoutViewModel
            {
                TotalAmount = totalAmount
            };

            ViewBag.SelectedItems = selectedCartItems;  
            return View("Checkout", checkoutModel);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == user.Id);

            if (cartItem == null)
            {
                TempData["Error"] = "Item not found.";
                return RedirectToAction("Index");
            }

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1;
            }
            else
            {
                _context.CartItems.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


















        // Process Payment (POST)
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(CheckoutViewModel model, List<int> selectedItems)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id && selectedItems.Contains(c.Id))
                .ToListAsync();

            if (user == null)
            {
                TempData["Error"] = "User not found. Please log in.";
                return RedirectToAction("Login", "Account");
            }

            // Ensure cartItems is not null
            if (cartItems == null || !cartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return RedirectToAction("Index", "Cart");
            }

            // Save the order before clearing the cart
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c => (c.Product?.Price ?? 0) * c.Quantity), 
                Status = "Pending",
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Price = c.Product?.Price ?? 0, 
                    Quantity = c.Quantity
                }).ToList()
            };


            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation");
        }


        // Order Confirmation Page
        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}

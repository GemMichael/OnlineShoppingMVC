using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingMVC.Data;
using OnlineShoppingMVC.Models;

namespace OnlineShoppingMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "true";
        }

        // Product List (Admin Only)
        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var products = _context.Products.ToList(); 
            return View(products);
        }

        // Show Create Form (Admin Only)
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            return View();
        }

        // Handle Create Form Submission 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // efine Image Folder Path
                    var imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    //  Ensure Directory Exists
                    if (!Directory.Exists(imagesFolderPath))
                    {
                        Directory.CreateDirectory(imagesFolderPath);
                    }

                    
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(imagesFolderPath, fileName);

                   
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                 
                    product.ImagePath = "/images/" + fileName;
                }
                else
                {
                    product.ImagePath = "/images/default.jpg"; 
                }


                _context.Products.Add(product);
                _context.SaveChanges(); 
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // ✏Show Edit Form (Admin Only)
        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var product = _context.Products.Find(id); 
            if (product == null) return NotFound();
            return View(product);
        }

        // Handle Edit Form Submission (With Image Upload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product updatedProduct, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var product = _context.Products.Find(updatedProduct.Id);
            if (product != null && ModelState.IsValid)
            {
                product.Name = updatedProduct.Name;
                product.Description = updatedProduct.Description;
                product.Price = updatedProduct.Price;

                // Save New Image 
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }


                    product.ImagePath = "/images/" + fileName;
                }

                _context.Products.Update(product);
                _context.SaveChanges(); 
                return RedirectToAction("Index");
            }
            return View(updatedProduct);
        }

        // Public product list (For All Users)
        public IActionResult PublicIndex()
        {
            var products = _context.Products.ToList(); 
            return View(products);
        }

        // View Product Details (For All Users)
        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id); 

            if (product == null)
            {
                return NotFound();
            }

            return View(product); 
        }

        // Delete Product (Admin Only)
        public IActionResult Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges(); 
            }
            return RedirectToAction("Index");
        }
    }
}

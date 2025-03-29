using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingMVC.Models;

namespace OnlineShoppingMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>  
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }  

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }




    }
}

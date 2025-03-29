using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public required string Address { get; set; }
    }
}

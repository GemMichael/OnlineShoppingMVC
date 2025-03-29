using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingMVC.Models
{
    public class ProfileViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}

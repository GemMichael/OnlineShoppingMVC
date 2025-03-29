using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingMVC.Models
{
    public class CheckoutViewModel
    {
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Cardholder Name is required")]
        public string CardholderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Card Number is required")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Invalid Card Number. Must be 16 digits.")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiry Date is required")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Invalid Expiry Date (MM/YY)")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV must be 3 digits")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "Invalid CVV")]
        public string CVV { get; set; } = string.Empty;
    }

}

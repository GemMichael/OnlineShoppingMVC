using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingMVC.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public required string UserId { get; set; } 

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; } 

        public int Quantity { get; set; }
    }
}

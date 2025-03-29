using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingMVC.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0m; 

        [Required]
        public int Quantity { get; set; } = 1; 

        public Order Order { get; set; } = null!; 
        public Product Product { get; set; } = null!;
    }
}

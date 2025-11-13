using System.ComponentModel.DataAnnotations.Schema;

namespace Cldv6212_RetailAppPartThree.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
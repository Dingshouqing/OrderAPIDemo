using System.ComponentModel.DataAnnotations;

namespace OrderWebAPI.DTOs
{
    public class OrderItemDto
    {
        [Required(ErrorMessage = "Product ID is required.")]
        [StringLength(50, ErrorMessage = "Product ID must be 50 characters.")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
}

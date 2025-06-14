using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderWebAPI.Model
{
    /// <summary>
    /// Order Item Model
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Order Id
        /// </summary>
        [Required]
        public Guid OrderId { get; set; }

        /// <summary>
        /// Product Id
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Quantity
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        /// <summary>
        /// Order reference
        /// </summary>
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}

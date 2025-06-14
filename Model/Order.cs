using System.ComponentModel.DataAnnotations;

namespace OrderWebAPI.Model
{
    /// <summary>
    /// Order Model
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Order Id
        /// </summary>
        [Key]
        public Guid OrderId { get; set; }

        /// <summary>
        /// Customer Name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Created At
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Order Items
        /// </summary>
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); 
    }
}

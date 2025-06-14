namespace OrderWebAPI.DTOs
{
    public class CreateOrderResponse
    {
        public Guid OrderId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<OrderItemResponse> OrderItems { get; set; } = new List<OrderItemResponse>();
    }
}

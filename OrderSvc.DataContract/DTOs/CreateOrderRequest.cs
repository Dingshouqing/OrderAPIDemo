namespace OrderSvc.DataContract.DTOs
{
    public class CreateOrderRequest
    {
        public Guid? OrderId    { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public IEnumerable<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}

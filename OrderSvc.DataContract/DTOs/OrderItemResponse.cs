namespace OrderSvc.DataContract.DTOs
{
    public class OrderItemResponse
    {
        public string ProductId { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}

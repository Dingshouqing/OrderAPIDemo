using OrderWebAPI.DTOs;

namespace OrderWebAPI.Services
{
    public interface IOrderService
    {
        Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);

        Task<CreateOrderResponse?> GetOrderAsync(Guid orderId);

        Task<IEnumerable<CreateOrderResponse>> GetAllOrdersAsync();
    }
}

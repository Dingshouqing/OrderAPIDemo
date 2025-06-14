using OrderSvc.DataContract.DTOs;

namespace OrderSvc.WebAPI.Services
{
    public interface IOrderService
    {
        Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);

        Task<CreateOrderResponse?> GetOrderAsync(Guid orderId);

        Task<IEnumerable<CreateOrderResponse>> GetAllOrdersAsync();
    }
}

using OrderSvc.DataContract.DTOs;

namespace OrderSvc.WebAPI.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Creates a new order based on the provided request
        /// </summary>
        /// <param name="request">The order creation request containing customer information and order items</param>
        /// <returns>A CreateOrderResponse containing the created order details</returns>
        /// <exception cref="InvalidOrderDataException">Thrown when the order data is invalid or duplicate</exception>
        Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);

        /// <summary>
        /// Retrieves an order by its unique identifier
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to retrieve</param>
        /// <returns>The order details if found, otherwise throws OrderNotFoundException</returns>
        /// <exception cref="OrderNotFoundException">Thrown when the order with the specified ID is not found</exception>
        Task<CreateOrderResponse?> GetOrderAsync(Guid orderId);

        /// <summary>
        /// Retrieves all orders from the database
        /// </summary>
        /// <returns>A collection of all orders in the system</returns>
        Task<IEnumerable<CreateOrderResponse>> GetAllOrdersAsync();
    }
}

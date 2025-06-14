using OrderSvc.DataContract.DTOs;
using OrderSvc.WebAPI.Exceptions;
using OrderSvc.WebAPI.Repositories;
using OrderSvc.WebAPI.Utilities;

namespace OrderSvc.WebAPI.Services.Concrete
{
    public class OrderService(IOrderRepository _orderRepository, ILogger<OrderService> _logger) : IOrderService
    {
        public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            try
            {
                // validate request payload
                request.ValidateCreateOrderRequest();

                _logger.LogInformation($"Creating order for customer: {request.CustomerName}");

                // Generate OrderId if not provided
                request.OrderId ??= Guid.NewGuid();

                // Check if order already exists
                if (await _orderRepository.ExistsAsync(request.OrderId.Value))
                {
                    throw new InvalidOrderDataException($"Order with ID {request.OrderId.Value} already exists.");
                }

                // Map DTO to entity
                var order = request.MapToOrder();

                // Save to Database
                var createdOrder = await _orderRepository.CreateAsync(order);

                // Map back to reponse DTO
                if (createdOrder != null)
                {
                    var response = createdOrder.MapToOrderResponse();

                    _logger.LogInformation("Successfully created order with ID: {OrderId}", response.OrderId);

                    return response;
                }
                else
                {
                    throw new InvalidOrderDataException("Order could not be created.");
                }
            }
            catch (InvalidOrderDataException invalidEx)
            {
                _logger.LogError(invalidEx, "Invalid order data for customer: {CustomerName}", request.CustomerName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for customer: {CustomerName}", request.CustomerName);
                throw;
            }
        }

        public async Task<CreateOrderResponse?> GetOrderAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Retrieving order with ID: {OrderId}", orderId);

                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order != null)
                {
                    return order.MapToOrderResponse();
                }
                else
                {
                    _logger.LogWarning("Order not found with ID: {OrderId}", orderId);
                    throw new OrderNotFoundException(orderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with ID: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<IEnumerable<CreateOrderResponse>> GetAllOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all orders");

                var orders = await _orderRepository.GetAllAsync();
                return orders.Select(o => o.MapToOrderResponse());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                throw;
            }
        }
    }
}

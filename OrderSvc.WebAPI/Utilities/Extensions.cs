using OrderSvc.DataContract.DTOs;
using OrderSvc.DataContract.Model;
using OrderSvc.WebAPI.Exceptions;

namespace OrderSvc.WebAPI.Utilities
{
    public static class Extensions
    {
        /// <summary>
        /// Maps a CreateOrderRequest DTO to an Order entity
        /// </summary>
        /// <param name="request">The order creation request containing customer and item information</param>
        /// <returns>A new Order entity with mapped properties and generated timestamps</returns>
        public static Order MapToOrder(this CreateOrderRequest request)
        {
            return new Order
            {
                OrderId = request.OrderId ?? Guid.NewGuid(),
                CustomerName = request.CustomerName.Trim(),
                CreatedAt = DateTime.UtcNow,
                OrderItems = request.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId.Trim(),
                    Quantity = item.Quantity,
                    OrderId = request.OrderId ?? Guid.NewGuid()
                }).ToList()
            };
        }

        /// <summary>
        /// Maps an Order entity to a CreateOrderResponse DTO
        /// </summary>
        /// <param name="order">The order entity to map</param>
        /// <returns>A CreateOrderResponse DTO with mapped order and item information</returns>
        public static CreateOrderResponse MapToOrderResponse(this Order order)
        {
            return new CreateOrderResponse
            {
                OrderId = order.OrderId,
                CustomerName = order.CustomerName,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems.Select(item => new OrderItemResponse
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };
        }

        /// <summary>
        /// Validates a CreateOrderRequest for required fields and business rules
        /// </summary>
        /// <param name="request">The order creation request to validate</param>
        /// <exception cref="InvalidOrderDataException">Thrown when validation fails due to missing or invalid data</exception>
        public static void ValidateCreateOrderRequest(this CreateOrderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                throw new InvalidOrderDataException("Customer name is required.");
            }

            if (request.OrderItems == null || !request.OrderItems.Any())
            {
                throw new InvalidOrderDataException("At least one order item is required.");
            }

            foreach (var item in request.OrderItems)
            {
                if (string.IsNullOrWhiteSpace(item.ProductId))
                {
                    throw new InvalidOrderDataException("Product ID is required for all items.");
                }

                if (item.Quantity <= 0)
                {
                    throw new InvalidOrderDataException("Quantity must be greater than zero for all items.");
                }
            }
        }
    }
}

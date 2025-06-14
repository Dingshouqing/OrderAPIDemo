using OrderWebAPI.DTOs;
using OrderWebAPI.Exceptions;
using OrderWebAPI.Model;

namespace OrderWebAPI.Utilities
{
    public static class Extensions
    {
        public static Order MapToOrder(this CreateOrderRequest request)
        {
            return new Order
            {
                OrderId = request.OrderId.Value,
                CustomerName = request.CustomerName.Trim(),
                CreatedAt = DateTime.UtcNow,
                OrderItems = request.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId.Trim(),
                    Quantity = item.Quantity,
                    OrderId = request.OrderId.Value
                }).ToList()
            };
        }

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

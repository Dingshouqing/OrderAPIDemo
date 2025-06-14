using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrderSvc.DataContract.DTOs;
using OrderSvc.WebAPI.Data;
using OrderSvc.WebAPI.Exceptions;
using OrderSvc.WebAPI.Repositories;
using OrderSvc.WebAPI.Repositories.Concrete;
using OrderSvc.WebAPI.Services;
using OrderSvc.WebAPI.Services.Concrete;

namespace OrderSvc.Tests.Services
{
    public class OrderServiceUnitTests : IDisposable
    {
        private readonly OrderDbContext _context;
        private readonly Mock<ILogger<OrderRepository>> _repositoryLoggerMock;
        private readonly Mock<ILogger<OrderService>> _serviceLoggerMock;
        private readonly IOrderRepository _repository;
        private readonly IOrderService _service;

        public OrderServiceUnitTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new OrderDbContext(options);
            _repositoryLoggerMock = new Mock<ILogger<OrderRepository>>();
            _serviceLoggerMock = new Mock<ILogger<OrderService>>();

            _repository = new OrderRepository(_context, _repositoryLoggerMock.Object);
            _service = new OrderService(_repository, _serviceLoggerMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidRequest_ReturnsCreatedOrder()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "John Doe",
                OrderItems = new List<OrderItemDto>
                {
                    new() { ProductId = "PROD001", Quantity = 2 },
                    new() { ProductId = "PROD002", Quantity = 1 }
                }
            };

            // Act
            var result = await _service.CreateOrderAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.OrderId);
            Assert.Equal("John Doe", result.CustomerName);
            Assert.Equal(2, result.OrderItems.Count);
            Assert.True(result.CreatedAt > DateTime.MinValue);
        }

        [Fact]
        public async Task CreateOrderAsync_WithSpecificOrderId_ReturnsOrderWithSpecifiedId()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var request = new CreateOrderRequest
            {
                OrderId = orderId,
                CustomerName = "Jane Smith",
                OrderItems = new List<OrderItemDto>
                {
                    new() { ProductId = "PROD003", Quantity = 3 }
                }
            };

            // Act
            var result = await _service.CreateOrderAsync(request);

            // Assert
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal("Jane Smith", result.CustomerName);
        }

        [Fact]
        public async Task CreateOrderAsync_DuplicateOrderId_ThrowsInvalidOrderDataException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var request1 = new CreateOrderRequest
            {
                OrderId = orderId,
                CustomerName = "Customer 1",
                OrderItems = new List<OrderItemDto>
                {
                    new() { ProductId = "PROD001", Quantity = 1 }
                }
            };

            var request2 = new CreateOrderRequest
            {
                OrderId = orderId,
                CustomerName = "Customer 2",
                OrderItems = new List<OrderItemDto>
                {
                    new() { ProductId = "PROD002", Quantity = 1 }
                }
            };

            // Act & Assert
            await _service.CreateOrderAsync(request1);
            var exception = await Assert.ThrowsAsync<InvalidOrderDataException>(
                () => _service.CreateOrderAsync(request2));

            Assert.Contains($"Order with ID {orderId} already exists", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateOrderAsync_InvalidCustomerName_ThrowsInvalidOrderDataException(string customerName)
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = customerName,
                OrderItems = new List<OrderItemDto>
            {
                new() { ProductId = "PROD001", Quantity = 1 }
            }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOrderDataException>(
                () => _service.CreateOrderAsync(request));

            Assert.Equal("Customer name is required.", exception.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_EmptyItems_ThrowsInvalidOrderDataException()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "John Doe",
                OrderItems = new List<OrderItemDto>()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOrderDataException>(
                () => _service.CreateOrderAsync(request));

            Assert.Equal("At least one order item is required.", exception.Message);
        }

        [Fact]
        public async Task CreateOrderAsync_NullItems_ThrowsInvalidOrderDataException()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "John Doe",
                OrderItems = null!
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOrderDataException>(
                () => _service.CreateOrderAsync(request));

            Assert.Equal("At least one order item is required.", exception.Message);
        }

        [Theory]
        [InlineData("", 1)]
        [InlineData("   ", 1)]
        [InlineData(null, 1)]
        [InlineData("PROD001", 0)]
        [InlineData("PROD001", -1)]
        public async Task CreateOrderAsync_InvalidOrderItem_ThrowsInvalidOrderDataException(
            string productId, int quantity)
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "John Doe",
                OrderItems = new List<OrderItemDto>
                {
                    new() { ProductId = productId, Quantity = quantity }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOrderDataException>(
                () => _service.CreateOrderAsync(request));

            Assert.True(exception.Message.Contains("Product ID is required") ||
                       exception.Message.Contains("Quantity must be greater than zero"));
        }

        [Fact]
        public async Task GetOrderAsync_ExistingOrder_ReturnsOrder()
        {
            // Arrange
            var createRequest = new CreateOrderRequest
            {
                CustomerName = "Test Customer",
                OrderItems = new List<OrderItemDto>
                {
                    new() { ProductId = "PROD001", Quantity = 1 }
                }
            };

            var createdOrder = await _service.CreateOrderAsync(createRequest);

            // Act
            var result = await _service.GetOrderAsync(createdOrder.OrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdOrder.OrderId, result.OrderId);
            Assert.Equal("Test Customer", result.CustomerName);
        }

        [Fact]
        public async Task GetOrderAsync_NonExistentOrder_ThrowsOrderNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrderNotFoundException>(
                () => _service.GetOrderAsync(nonExistentId));

            Assert.Contains($"Order with ID {nonExistentId} was not found", exception.Message);
        }

        [Fact]
        public async Task GetAllOrdersAsync_WithMultipleOrders_ReturnsAllOrdersOrderedByCreatedAt()
        {
            // Arrange
            var request1 = new CreateOrderRequest
            {
                CustomerName = "Customer 1",
                OrderItems = new List<OrderItemDto>
                {
                    new() { ProductId = "PROD001", Quantity = 1 }
                },
            };

            var request2 = new CreateOrderRequest
            {
                CustomerName = "Customer 2",
                OrderItems = new List<OrderItemDto>
                {
                    new() { ProductId = "PROD002", Quantity = 2 }
                },
            };

            await _service.CreateOrderAsync(request1);
            await _service.CreateOrderAsync(request2);

            // Act
            var result = await _service.GetAllOrdersAsync();

            // Assert
            var orders = result.ToList();
            Assert.Equal(2, orders.Count);

            // Orders should be ordered by CreatedAt descending (most recent first)
            Assert.True(orders[0].CreatedAt >= orders[1].CreatedAt);
            Assert.Equal("Customer 2", orders[0].CustomerName);
            Assert.Equal("Customer 1", orders[1].CustomerName);
        }

        [Fact]
        public async Task GetAllOrdersAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _service.GetAllOrdersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

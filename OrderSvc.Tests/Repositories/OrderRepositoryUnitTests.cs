using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrderSvc.DataContract.Model;
using OrderSvc.WebAPI.Data;
using OrderSvc.WebAPI.Repositories;
using OrderSvc.WebAPI.Repositories.Concrete;

namespace OrderSvc.Tests.Repositories
{
    public class OrderRepositoryUnitTests : IDisposable
    {
        private readonly OrderDbContext _context;
        private readonly Mock<ILogger<OrderRepository>> _loggerMock;
        private readonly IOrderRepository _repository;

        public OrderRepositoryUnitTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new OrderDbContext(options);
            _loggerMock = new Mock<ILogger<OrderRepository>>();
            _repository = new OrderRepository(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidOrder_ReturnsCreatedOrder()
        {
            // Arrange
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>
            {
                new() { ProductId = "PROD001", Quantity = 2 },
                new() { ProductId = "PROD002", Quantity = 1 }
            }
            };

            // Act
            var result = await _repository.CreateAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.OrderId, result.OrderId);
            Assert.Equal(order.CustomerName, result.CustomerName);
            Assert.Equal(2, result.OrderItems.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingOrder_ReturnsOrderWithItems()
        {
            // Arrange
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>
            {
                new() { ProductId = "PROD001", Quantity = 2 }
            }
            };

            await _repository.CreateAsync(order);

            // Act
            var result = await _repository.GetByIdAsync(order.OrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.OrderId, result.OrderId);
            Assert.Equal(order.CustomerName, result.CustomerName);
            Assert.Single(result.OrderItems);
            Assert.Equal("PROD001", result.OrderItems.First().ProductId);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentOrder_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleOrders_ReturnsAllOrdersOrderedByCreatedAt()
        {
            // Arrange
            var order1 = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Customer 1",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                OrderItems = new List<OrderItem>
            {
                new() { ProductId = "PROD001", Quantity = 1 }
            }
            };

            var order2 = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Customer 2",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                OrderItems = new List<OrderItem>
            {
                new() { ProductId = "PROD002", Quantity = 2 }
            }
            };

            await _repository.CreateAsync(order1);
            await _repository.CreateAsync(order2);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            var orders = result.ToList();
            Assert.Equal(2, orders.Count);

            // Should be ordered by CreatedAt descending
            Assert.True(orders[0].CreatedAt >= orders[1].CreatedAt);
            Assert.Equal("Customer 2", orders[0].CustomerName);
            Assert.Equal("Customer 1", orders[1].CustomerName);
        }

        [Fact]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_ExistingOrder_ReturnsUpdatedOrder()
        {
            // Arrange
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Original Customer",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>
            {
                new() { ProductId = "PROD001", Quantity = 1 }
            }
            };

            await _repository.CreateAsync(order);

            // Modify the order
            order.CustomerName = "Updated Customer";

            // Act
            var result = await _repository.UpdateAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Customer", result.CustomerName);

            // Verify the change persisted
            var retrieved = await _repository.GetByIdAsync(order.OrderId);
            Assert.Equal("Updated Customer", retrieved!.CustomerName);
        }

        [Fact]
        public async Task DeleteAsync_ExistingOrder_ReturnsTrue()
        {
            // Arrange
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>
            {
                new() { ProductId = "PROD001", Quantity = 1 }
            }
            };

            await _repository.CreateAsync(order);

            // Act
            var result = await _repository.DeleteAsync(order.OrderId);

            // Assert
            Assert.True(result);

            // Verify the order was deleted
            var retrieved = await _repository.GetByIdAsync(order.OrderId);
            Assert.Null(retrieved);
        }

        [Fact]
        public async Task DeleteAsync_NonExistentOrder_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_ExistingOrder_ReturnsTrue()
        {
            // Arrange
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>
            {
                new() { ProductId = "PROD001", Quantity = 1 }
            }
            };

            await _repository.CreateAsync(order);

            // Act
            var result = await _repository.ExistsAsync(order.OrderId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_NonExistentOrder_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateAsync_OrderWithItems_CreatesOrderAndItemsCorrectly()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                OrderId = orderId,
                CustomerName = "Test Customer",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>
            {
                new() { ProductId = "PROD001", Quantity = 2, OrderId = orderId },
                new() { ProductId = "PROD002", Quantity = 3, OrderId = orderId }
            }
            };

            // Act
            var result = await _repository.CreateAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(2, result.OrderItems.Count);

            // Verify items are properly linked
            foreach (var item in result.OrderItems)
            {
                Assert.Equal(orderId, item.OrderId);
                Assert.NotNull(item.Order);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
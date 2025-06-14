using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OrderSvc.DataContract;
using OrderSvc.DataContract.DTOs;
using OrderSvc.WebAPI.Data;
using OrderWebAPI;
using System.Text;
using System.Text.Json;

namespace OrderSvc.Tests.IntegrationTests
{
    public class OrdersApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrdersApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(Constrants.TestEnvironment);

                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<OrderDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // To remove specific Entity Framework Core database providers from the service collection in your test setup,
                    // use RemoveAll for their corresponding DbContextOptions types, e.g.:
                    services.RemoveAll(typeof(DbContextOptions<OrderDbContext>));
                    // If you want to ensure removal of all providers, you can also remove their service descriptors directly:
                    services.RemoveAll(typeof(Microsoft.EntityFrameworkCore.Storage.IRelationalConnection)); // For relational providers like Sqlite
                    services.RemoveAll(typeof(Microsoft.EntityFrameworkCore.Storage.IDatabaseProvider)); // For all providers

                    // Add in-memory database for testing
                    services.AddDbContext<OrderDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));

                    // Reduce logging noise in tests
                    services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
                });
            });

            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        [Fact]
        public async Task CreateOrder_ValidRequest_ReturnsCreatedOrder()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "Integration Test Customer",
                OrderItems = new List<OrderItemDto>
            {
                new() { ProductId = "PROD001", Quantity = 2 },
                new() { ProductId = "PROD002", Quantity = 1 }
            }
            };

            var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateOrderResponse>>(
                responseContent, _jsonOptions);

            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.NotEqual(Guid.Empty, apiResponse.Data.OrderId);
            Assert.Equal("Integration Test Customer", apiResponse.Data.CustomerName);
            Assert.Equal(2, apiResponse.Data.OrderItems.Count);
        }

        [Fact]
        public async Task CreateOrder_WithSpecificOrderId_ReturnsCreatedOrderWithSpecifiedId()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var request = new CreateOrderRequest
            {
                OrderId = orderId,
                CustomerName = "Test Customer With ID",
                OrderItems = new List<OrderItemDto>
            {
                new() { ProductId = "PROD003", Quantity = 3 }
            }
            };

            var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateOrderResponse>>(
                responseContent, _jsonOptions);

            Assert.NotNull(apiResponse);
            Assert.Equal(orderId, apiResponse.Data!.OrderId);
        }

        [Fact]
        public async Task CreateOrder_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "", // Invalid: empty customer name
                OrderItems = new List<OrderItemDto>()
            };

            var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
                responseContent, _jsonOptions);

            Assert.NotNull(apiResponse);
            Assert.False(apiResponse.Success);
            Assert.NotEmpty(apiResponse.Message);
        }

        [Fact]
        public async Task GetOrder_NonExistentOrder_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/orders/{nonExistentId}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(
                responseContent, _jsonOptions);

            Assert.NotNull(apiResponse);
            Assert.False(apiResponse.Success);
            Assert.Contains("was not found", apiResponse.Message);
        }

        [Fact]
        public async Task CreateOrder_NullRequest_ReturnsBadRequest()
        {
            // Arrange
            var content = new StringContent("null", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_InvalidJsonFormat_ReturnsBadRequest()
        {
            // Arrange
            var content = new StringContent("{ invalid json }", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/orders", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

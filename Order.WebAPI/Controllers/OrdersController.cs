using Microsoft.AspNetCore.Mvc;
using OrderSvc.DataContract.DTOs;
using OrderSvc.WebAPI.Exceptions;
using OrderSvc.WebAPI.Services;

namespace OrderSvc.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController(IOrderService _orderService, ILogger<OrdersController> _logger) : ControllerBase
    {
        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="request">The order creation request</param>
        /// <returns>The created order details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CreateOrderResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                _logger.LogInformation("Received request to create order for customer: {CustomerName}", request?.CustomerName);

                if (request == null)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("Request body is required."));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                    .SelectMany(x => x.Value?.Errors.Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>())
                    .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Validation failed.", errors));
                }

                var orderResponse = await _orderService.CreateOrderAsync(request);

                var response = ApiResponse<CreateOrderResponse>.SuccessResult(orderResponse, "Order created successfully.");

                return CreatedAtAction(nameof(GetOrder), new { Id = orderResponse.OrderId }, response);
            }
            catch (InvalidOrderDataException ex)
            {
                _logger.LogWarning(ex, "Invalid order data provided");
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (OrderServiceException ex)
            {
                _logger.LogError(ex, "Order service error occurred");
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating order");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An unexpected error occurred."));
            }
        }

        /// <summary>
        /// Gets an order by ID
        /// </summary>
        /// <param name="id">The order ID</param>
        /// <returns>The order details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CreateOrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            try
            {
                _logger.LogInformation("Received request to get order with ID: {OrderId}", id);

                var orderResponse = await _orderService.GetOrderAsync(id);
                if (orderResponse == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Order not found."));
                }

                var response = ApiResponse<CreateOrderResponse>.SuccessResult(
                    orderResponse,
                    "Order retrieved successfully."
                );

                return Ok(response);
            }
            catch (OrderNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found with ID: {OrderId}", id);
                return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving order with ID: {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An unexpected error occurred."));
            }
        }

        /// <summary>
        /// Gets all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CreateOrderResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                _logger.LogInformation("Received request to get all orders");

                var orders = await _orderService.GetAllOrdersAsync();
                var response = ApiResponse<IEnumerable<CreateOrderResponse>>.SuccessResult(
                    orders,
                    "Orders retrieved successfully."
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all orders");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An unexpected error occurred."));
            }
        }
    }
}

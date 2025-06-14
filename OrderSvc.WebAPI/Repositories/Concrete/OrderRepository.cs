using Microsoft.EntityFrameworkCore;
using OrderSvc.DataContract.Model;
using OrderSvc.WebAPI.Data;

namespace OrderSvc.WebAPI.Repositories.Concrete
{
    public class OrderRepository(OrderDbContext _context, ILogger<OrderRepository> _logger) : IOrderRepository
    {
        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Retrieving order with ID: {OrderId}", orderId);

                return await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with ID: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all orders");

                return await _context.Orders
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                throw;
            }
        }

        public async Task<Order> CreateAsync(Order order)
        {
            try
            {
                _logger.LogInformation("Creating new order with ID: {OrderId}", order.OrderId);

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Reload the order with items to return complete entity
                return await GetByIdAsync(order.OrderId) ?? order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order with ID: {OrderId}", order.OrderId);
                throw;
            }
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            try
            {
                _logger.LogInformation("Updating order with ID: {OrderId}", order.OrderId);

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order with ID: {OrderId}", order.OrderId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Deleting order with ID: {OrderId}", orderId);

                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return false;
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order with ID: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Guid orderId)
        {
            try
            {
                return await _context.Orders.AnyAsync(o => o.OrderId == orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if order exists with ID: {OrderId}", orderId);
                throw;
            }
        }
    }
}

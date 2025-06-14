using OrderSvc.DataContract.Model;

namespace OrderSvc.WebAPI.Repositories
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <returns>Order instance if found, otherwise null</returns>
        Task<Order?> GetByIdAsync(Guid orderId);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        Task<IEnumerable<Order>> GetAllAsync();

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="order">The order to create</param>
        /// <returns>The created order instance</returns>
        Task<Order?> CreateAsync(Order order);

        /// <summary>
        /// Update an existing order
        /// </summary>
        /// <param name="order"></param>
        /// <returns>The updated order instance</returns>
        Task<Order?> UpdateAsync(Order order);

        /// <summary>
        /// Delete an order by id
        /// </summary>
        /// <param name="orderId">order Id</param>
        /// <returns>true if the order was deleted, otherwise false</returns>
        Task<bool> DeleteAsync(Guid orderId);

        /// <summary>
        /// Check if an order exists by id
        /// </summary>
        /// <param name="orderId">The order Id to check</param>
        /// <returns>True if the order exists, otherwise false</returns>
        Task<bool> ExistsAsync(Guid orderId);
    }
}

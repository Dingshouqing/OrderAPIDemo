namespace OrderSvc.WebAPI.Exceptions
{
    /// <summary>
    /// Base exception class for order service related errors
    /// </summary>
    public class OrderServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the OrderServiceException class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public OrderServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrderServiceException class with a specified error message and inner exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public OrderServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when an order is not found
    /// </summary>
    public class OrderNotFoundException : OrderServiceException
    {
        /// <summary>
        /// Initializes a new instance of the OrderNotFoundException class with the specified order ID
        /// </summary>
        /// <param name="orderId">The ID of the order that was not found</param>
        public OrderNotFoundException(Guid orderId) : base($"Order with ID {orderId} was not found.")
        {
        }
    }

    /// <summary>
    /// Exception thrown when order data is invalid or does not meet business rules
    /// </summary>
    public class InvalidOrderDataException : OrderServiceException
    {
        /// <summary>
        /// Initializes a new instance of the InvalidOrderDataException class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the validation error</param>
        public InvalidOrderDataException(string message) : base(message)
        {
        }
    }
}

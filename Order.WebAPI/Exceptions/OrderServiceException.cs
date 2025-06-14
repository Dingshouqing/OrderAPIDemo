namespace OrderSvc.WebAPI.Exceptions
{
    public class OrderServiceException : Exception
    {
        public OrderServiceException(string message) : base(message)
        {
        }

        public OrderServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class OrderNotFoundException : OrderServiceException
    {
        public OrderNotFoundException(Guid orderId) : base($"Order with ID {orderId} was not found.")
        {
        }
    }

    public class InvalidOrderDataException : OrderServiceException
    {
        public InvalidOrderDataException(string message) : base(message)
        {
        }
    }
}

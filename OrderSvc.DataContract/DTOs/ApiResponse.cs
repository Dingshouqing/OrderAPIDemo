namespace OrderSvc.DataContract.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        List<string> Errors { get; set; } = new();

        /// <summary>
        /// Creates a successful API response with data and optional message
        /// </summary>
        /// <param name="data">The response data</param>
        /// <param name="message">Optional success message</param>
        /// <returns>A successful ApiResponse instance</returns>
        public static ApiResponse<T> SuccessResult(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Creates an error API response with message and optional error details
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="errors">Optional list of detailed error messages</param>
        /// <returns>An error ApiResponse instance</returns>
        public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}

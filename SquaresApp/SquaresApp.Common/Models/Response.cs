namespace SquaresApp.Common.Models
{
    /// <summary>
    /// Generic Response type for all API responses
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Status of the response
        /// </summary>
        /// <example>true</example>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Message part for the response
        /// </summary>
        /// <example>Successful</example>
        public string Message { get; set; }

        /// <summary>
        /// Data part of the response
        /// </summary>
        /// <example>{...}</example>
        public T Data { get; set; }
    }
}

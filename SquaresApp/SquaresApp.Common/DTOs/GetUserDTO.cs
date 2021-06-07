namespace SquaresApp.Common.DTOs
{
    /// <summary>
    /// User's detail DTO
    /// </summary> 
    public class GetUserDTO : UserDTO
    {
        /// <summary>
        /// Id of the User
        /// </summary>
        /// <example>7</example>
        public long Id { get; set; }
    }
}

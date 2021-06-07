namespace SquaresApp.Common.DTOs
{
    /// <summary>
    /// User's detail DTO
    /// </summary> 
    public class UserDTO
    {

        /// <summary>
        /// Username of the User
        /// </summary>
        /// <example>SuperAdmin</example> 
        public string Username { get; set; }

        /// <summary>
        /// Password of the User
        /// </summary>
        /// <example>StrongPass123</example> 
        public string Password { get; set; }
    }
}

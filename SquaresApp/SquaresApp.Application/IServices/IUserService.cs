using SquaresApp.Common.DTOs;
using System.Threading.Tasks;

namespace SquaresApp.Application.IServices
{
    public interface IUserService
    {
        /// <summary>
        /// get user detail by username and password
        /// </summary>
        /// <param name="userDTO"></param> 
        /// <returns></returns>
        Task<(GetUserDTO getUserDTO, string errorMessage)> GetUserAsync(UserDTO userDTO);

        /// <summary>
        /// create / add  a new user
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        Task<(GetUserDTO getUserDTO, string errorMessage)> AddUserAsync(UserDTO userDTO);

    }
}

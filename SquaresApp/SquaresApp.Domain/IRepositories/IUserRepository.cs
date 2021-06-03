using SquaresApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Domain.IRepositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// get user detail by username and hashedPassword
        /// </summary>
        /// <param name="username"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        Task<(User user, string errorMessage)> GetUserAsync(string username, string hashedPassword);
 
        /// <summary>
        /// create / add a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(User user, string errorMessage)> AddUserAsync(User user);

        /// <summary>
        /// check existance of a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<bool> CheckUserExistanceAsync(string username);
    }
}

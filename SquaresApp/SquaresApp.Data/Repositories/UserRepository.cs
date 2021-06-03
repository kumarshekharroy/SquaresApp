using Microsoft.EntityFrameworkCore;
using SquaresApp.Common.Constants;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Data.Context;
using SquaresApp.Domain.IRepositories;
using SquaresApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SquaresAppDBContext _squaresAppDBContext;

        public UserRepository(SquaresAppDBContext squaresAppDBContext)
        {
            _squaresAppDBContext = squaresAppDBContext;
        }


        /// <summary>
        /// create / add  a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(User user, string errorMessage)> AddUserAsync(User user)
        {
            var userExists = await CheckUserExistanceAsync(user.Username);
            if (userExists)
            {
                return (user: default, errorMessage: "Username already exists.");
            }
             
            await _squaresAppDBContext.Users.AddAsync(user);

            if (await _squaresAppDBContext.SaveChangesAsync() == 0)
            {
                return (user: default, errorMessage: ConstantValues.UnexpectedErrorMessage);
            }

            return (user: user, errorMessage: string.Empty);
        }


        /// <summary>
        /// check existance of a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> CheckUserExistanceAsync(string username)
        {
            return await _squaresAppDBContext.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower());
        }


        /// <summary>
        /// get user detail by username and hashedPassword
        /// </summary>
        /// <param name="username"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public async Task<(User user, string errorMessage)> GetUserAsync(string username, string hashedPassword)
        {
            var user = await _squaresAppDBContext.Users.FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower() && x.Password == hashedPassword);
            
            if (user is null)
            {
                return (user: default, errorMessage: "Username and password combination is incorrect.");
            }

            return (user: user, errorMessage: string.Empty);
        }
         
    }
}

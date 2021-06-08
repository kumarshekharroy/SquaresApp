using Microsoft.EntityFrameworkCore;
using SquaresApp.Common.Constants;
using SquaresApp.Data.Context;
using SquaresApp.Data.IRepositories;
using SquaresApp.Data.Models;
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
            var userExists = await _squaresAppDBContext.Users.AnyAsync(x => x.Username.ToLower() == user.Username.ToLower());
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

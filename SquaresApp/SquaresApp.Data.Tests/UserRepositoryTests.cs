using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Common.Helpers;
using SquaresApp.Data.Repositories;
using SquaresApp.Data.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.Data.Tests
{
    public class UserRepositoryTests : RepositoryBase
    {
        /// <summary>
        /// AddUser Test For Duplicate username
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddUser_UserAlreadyExist()
        {

            //Arrange   
            var user = new User() { Username = "Username", Password = StringHelper.GenerateRandomString(length: 10) };
            var expectedMessage = "Username already exists.";

            var userRepository = new UserRepository(_squaresAppDBContext);


            //Act
            var result = await userRepository.AddUserAsync(user);


            //Assert    
            Assert.Null(result.user);
            Assert.NotEmpty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedMessage);
        }

        /// <summary>
        /// AddUser Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddUser_Successful()
        {

            //Arrange   
            var user = new User() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };


            var userRepository = new UserRepository(_squaresAppDBContext);


            //Act
            var result = await userRepository.AddUserAsync(user);
            var userInDB = _squaresAppDBContext.Users.FirstOrDefault(x => x.Username == user.Username);

            //Assert    
            Assert.NotNull(result.user);
            Assert.NotNull(userInDB);
            Assert.True(result.user.Id>0);
            Assert.Equal(user.Username, userInDB.Username);
            Assert.Empty(result.errorMessage);
        }

        /// <summary>
        /// GetUser Test For Invalid Credentials
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUser_InvalidCredentials()
        {

            //Arrange   
            var user = new User() { Username = "Username", Password = StringHelper.GenerateRandomString(length: 10) };
            var expectedMessage = "Username and password combination is incorrect.";

            var userRepository = new UserRepository(_squaresAppDBContext);


            //Act
            var result = await userRepository.GetUserAsync(user.Username, user.Password);


            //Assert    
            Assert.Null(result.user);
            Assert.NotEmpty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedMessage);
        }

        /// <summary>
        /// GetUser Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUser_Successful()
        {

            //Arrange   
            var user = new User() { Username = "Username", Password = "123Abc".ToSHA256() };


            var userRepository = new UserRepository(_squaresAppDBContext);


            //Act
            var result = await userRepository.GetUserAsync(user.Username, user.Password);


            //Assert    
            Assert.NotNull(result.user);
            Assert.Equal(result.user.Username, user.Username);
            Assert.Empty(result.errorMessage);
        }
    }
}

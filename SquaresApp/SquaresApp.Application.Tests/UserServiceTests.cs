using Moq;
using SquaresApp.Application.Services;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.Helpers;
using SquaresApp.Data.IRepositories;
using SquaresApp.Data.Models;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.Application.Tests
{
    public class UserServiceTests : ServiceBase
    {
        private readonly Mock<IUserRepository> _mockedUserRepository;
        public UserServiceTests()
        {
            _mockedUserRepository = new Mock<IUserRepository>();
        }


        /// <summary>
        /// AddUser Test For Invalid UserName
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddUser_InvalidUserName()
        {

            //Arrange   
            var userDTO = new UserDTO() { Username = default, Password = StringHelper.GenerateRandomString(length: 10) };
            var expectedResult = (default(GetUserDTO), errorMessage: "Invalid username.");

            _mockedUserRepository.Setup(obj => obj.AddUserAsync(It.IsAny<User>())).ReturnsAsync((default(User), string.Empty)).Verifiable();

            var userService = new UserService(_mockedUserRepository.Object, _mapper);


            //Act
            var result = await userService.AddUserAsync(userDTO);


            //Assert   
            _mockedUserRepository.Verify(obj => obj.AddUserAsync(It.IsAny<User>()), Times.Never());
            Assert.Null(result.getUserDTO);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);

        }

        /// <summary>
        /// AddUser Test For Invalid Password
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddUser_InvalidPassword()
        {

            //Arrange   
            var userDTO = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = default };
            var expectedResult = (default(GetUserDTO), errorMessage: "Invalid password.");

            _mockedUserRepository.Setup(obj => obj.AddUserAsync(It.IsAny<User>())).ReturnsAsync((default(User), string.Empty)).Verifiable();

            var userService = new UserService(_mockedUserRepository.Object, _mapper);


            //Act
            var result = await userService.AddUserAsync(userDTO);


            //Assert   
            _mockedUserRepository.Verify(obj => obj.AddUserAsync(It.IsAny<User>()), Times.Never());
            Assert.Null(result.getUserDTO);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);

        }

        /// <summary>
        /// AddUser Test For Some failing some validation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddUser_FailedSomeValidation()
        {

            //Arrange   
            var userDTO = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };
            var expectedResult = (default(GetUserDTO), errorMessage: "someError message.");

            _mockedUserRepository.Setup(obj => obj.AddUserAsync(It.IsAny<User>())).ReturnsAsync((default(User), expectedResult.errorMessage)).Verifiable();

            var userService = new UserService(_mockedUserRepository.Object, _mapper);


            //Act
            var result = await userService.AddUserAsync(userDTO);


            //Assert   
            _mockedUserRepository.Verify(obj => obj.AddUserAsync(It.IsAny<User>()), Times.Once());
            Assert.Null(result.getUserDTO);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);

        }

        /// <summary>
        /// AddUser Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddUser_Successful()
        {

            //Arrange   
            var userDTO = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };
            var expectedResult = (getUserDto:new GetUserDTO { Id = 1, Username = userDTO.Username }, errorMessage: string.Empty);

            _mockedUserRepository.Setup(obj => obj.AddUserAsync(It.IsAny<User>())).ReturnsAsync((new User { Id = expectedResult.getUserDto.Id, Username = expectedResult.getUserDto.Username }, expectedResult.errorMessage)).Verifiable();

            var userService = new UserService(_mockedUserRepository.Object, _mapper);


            //Act
            var result = await userService.AddUserAsync(userDTO);


            //Assert   
            _mockedUserRepository.Verify(obj => obj.AddUserAsync(It.IsAny<User>()), Times.Once());
            Assert.NotNull(result.getUserDTO);
            Assert.Empty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);
            Assert.Equal(result.getUserDTO.Id, expectedResult.getUserDto.Id);
            Assert.Equal(result.getUserDTO.Username, expectedResult.getUserDto.Username);

        }





        /// <summary>
        /// GetUser Test For Invalid UserName
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUser_InvalidUserName()
        {

            //Arrange   
            var userDTO = new UserDTO() { Username = default, Password = StringHelper.GenerateRandomString(length: 10) };
            var expectedResult = (default(GetUserDTO), errorMessage: "Invalid username.");

            _mockedUserRepository.Setup(obj => obj.GetUserAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((default(User), string.Empty)).Verifiable();

            var userService = new UserService(_mockedUserRepository.Object, _mapper);


            //Act
            var result = await userService.GetUserAsync(userDTO);


            //Assert   
            _mockedUserRepository.Verify(obj => obj.GetUserAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.Null(result.getUserDTO);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);

        }

        /// <summary>
        /// GetUser Test For Invalid Password
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUser_InvalidPassword()
        {

            //Arrange   
            var userDTO = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = default };
            var expectedResult = (default(GetUserDTO), errorMessage: "Invalid password.");

            _mockedUserRepository.Setup(obj => obj.GetUserAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((default(User), string.Empty)).Verifiable();

            var userService = new UserService(_mockedUserRepository.Object, _mapper);


            //Act
            var result = await userService.GetUserAsync(userDTO);


            //Assert   
            _mockedUserRepository.Verify(obj => obj.GetUserAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.Null(result.getUserDTO);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);

        }

        /// <summary>
        /// GetUser Test For Some failing some validation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUser_FailedSomeValidation()
        {

            //Arrange   
            var userDTO = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };
            var expectedResult = (default(GetUserDTO), errorMessage:"someError message.");

            _mockedUserRepository.Setup(obj => obj.GetUserAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((default(User), expectedResult.errorMessage)).Verifiable();

            var userService = new UserService(_mockedUserRepository.Object, _mapper);


            //Act
            var result = await userService.GetUserAsync(userDTO);


            //Assert   
            _mockedUserRepository.Verify(obj => obj.GetUserAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.Null(result.getUserDTO);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);

        }

        /// <summary>
        /// GetUser Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUser_Successful()
        {

            //Arrange   
            var userDTO = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };
            var expectedResult = (getUserDto:new GetUserDTO { Id = 1, Username = userDTO.Username }, errorMessage: string.Empty);

            _mockedUserRepository.Setup(obj => obj.GetUserAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((new User { Id = expectedResult.getUserDto.Id, Username = expectedResult.getUserDto.Username }, expectedResult.errorMessage)).Verifiable();

            var userService = new UserService(_mockedUserRepository.Object, _mapper);


            //Act
            var result = await userService.GetUserAsync(userDTO);


            //Assert   
            _mockedUserRepository.Verify(obj => obj.GetUserAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.NotNull(result.getUserDTO);
            Assert.Empty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);
            Assert.Equal(result.getUserDTO.Id, expectedResult.getUserDto.Id);
            Assert.Equal(result.getUserDTO.Username, expectedResult.getUserDto.Username);

        }
    }
}

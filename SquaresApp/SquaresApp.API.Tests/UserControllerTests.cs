﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SquaresApp.API.Controllers.v1;
using SquaresApp.Application.IServices;
using SquaresApp.Common.Constants;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.Helpers;
using SquaresApp.Common.Models;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.API.Tests
{
    public class UserControllerTests
    {

        private readonly Mock<IUserService> _mockedUserService;
        private readonly AppSettings _appSettings;
        public UserControllerTests()
        {
            _mockedUserService = new Mock<IUserService>();
            _appSettings = new AppSettings() { JWTConfig = new JWTConfig { Secret = StringHelper.GenerateRandomString(length: 150) } };

        }


        /// <summary>
        /// Registration Method Test when payload is null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterTest_NullPayload()
        {
            //Arrange   
            var payload = default(UserDTO);
            const string expectedErrorMessage = "Invalid registration payload.";
            var userController = new UserController(_mockedUserService.Object, _appSettings);


            //Act
            var result = await userController.Register(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedUserService.Verify(obj => obj.AddUserAsync(It.IsAny<UserDTO>()), Times.Never());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, expectedErrorMessage);
        }

        /// <summary>
        /// Registration Method Test when payload is not null but registration failed
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterTest_NotNullPayloadButRegFailed()
        {
            //Arrange    
            var payload = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };
            var returnData = (default(GetUserDTO), "Failed due to some reason");
            _mockedUserService.Setup(obj => obj.AddUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(returnData).Verifiable();
            var userController = new UserController(_mockedUserService.Object, _appSettings);


            //Act
            var result = await userController.Register(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedUserService.Verify(obj => obj.AddUserAsync(It.IsAny<UserDTO>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, returnData.Item2);
        }


        /// <summary>
        /// Registration Method Test for successful registration
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterTest_RegSuccessful()
        {
            //Arrange   
            var payload = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };
            var returnData = (new GetUserDTO { Id = 1 }, string.Empty);
            _mockedUserService.Setup(obj => obj.AddUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(returnData).Verifiable();
            var userController = new UserController(_mockedUserService.Object, _appSettings);


            //Act
            var result = await userController.Register(payload);
            var objectResult = result as ObjectResult;


            //Assert   
            _mockedUserService.Verify(obj => obj.AddUserAsync(It.IsAny<UserDTO>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            Assert.True((objectResult.Value as Response<GetUserDTO>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<GetUserDTO>)?.Data);
            Assert.Equal((objectResult.Value as Response<GetUserDTO>).Data.Id, returnData.Item1.Id);

        }

        /// <summary>
        /// Login Method Test when payload is null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoginTest_NullPayload()
        {
            //Arrange   
            var payload = default(UserDTO);
            const string expectedErrorMessage = "Invalid authentication payload.";
            var userController = new UserController(_mockedUserService.Object, _appSettings);


            //Act
            var result = await userController.Login(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedUserService.Verify(obj => obj.GetUserAsync(It.IsAny<UserDTO>()), Times.Never());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, expectedErrorMessage);
        }

        /// <summary>
        /// Login Method Test when payload is not null but supplied credential is invalid
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoginTest_NotNullPayloadButInvalidCredential()
        {
            //Arrange    
            var payload = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };
            var returnData = (default(GetUserDTO), "Failed due to some reason");
            _mockedUserService.Setup(obj => obj.GetUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(returnData).Verifiable();
            var userController = new UserController(_mockedUserService.Object, _appSettings);


            //Act
            var result = await userController.Login(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedUserService.Verify(obj => obj.GetUserAsync(It.IsAny<UserDTO>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, returnData.Item2);
        }


        /// <summary>
        /// Login Method Test when payload is not null and supplied credential is valid but jwt secret missing from appsettings
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoginTest_NotNullPayloadValidCredentialButJWTSecretMissing()
        {
            //Arrange    
            var payload = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };
            var returnData = (new GetUserDTO { Id = 1, Username = StringHelper.GenerateRandomString(length: 10) }, string.Empty);
            _mockedUserService.Setup(obj => obj.GetUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(returnData).Verifiable();
            var appSettingWithNoJwtSecret = new AppSettings();
            var userController = new UserController(_mockedUserService.Object, appSettingWithNoJwtSecret);


            //Act
            var result = await userController.Login(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedUserService.Verify(obj => obj.GetUserAsync(It.IsAny<UserDTO>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, ConstantValues.UnexpectedErrorMessage);
        }

        /// <summary>
        /// Login Method Test when all good
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoginTest_Successful()
        {
            //Arrange    
            var payload = new UserDTO() { Username = StringHelper.GenerateRandomString(length: 10), Password = StringHelper.GenerateRandomString(length: 10) };
            var returnData = (new GetUserDTO { Id = 1, Username = StringHelper.GenerateRandomString(length: 10) }, string.Empty);
            const string expectedMessage = "Authentication successful.";
            _mockedUserService.Setup(obj => obj.GetUserAsync(It.IsAny<UserDTO>())).ReturnsAsync(returnData).Verifiable();
            var userController = new UserController(_mockedUserService.Object, _appSettings);


            //Act
            var result = await userController.Login(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedUserService.Verify(obj => obj.GetUserAsync(It.IsAny<UserDTO>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            Assert.True((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, expectedMessage);
            Assert.NotNull((objectResult.Value as Response<string>)?.Data);
        }
    }
}

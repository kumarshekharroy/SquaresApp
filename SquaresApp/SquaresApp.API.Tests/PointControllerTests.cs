using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using SquaresApp.API.Controllers.v1;
using SquaresApp.Application.IServices;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.API.Tests
{
    public class PointControllerTests : SecuredControllerBase
    {

        private readonly Mock<IPointService> _mockedPointService;
        public PointControllerTests()
        {
            _mockedPointService = new Mock<IPointService>();
        }



        /// <summary>
        /// Get all Points Method Test when all good
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetPointsTest_Successful()
        {
            //Arrange     
            const string expectedMessage = "Successfully fetched.";
            var returnData = Array.Empty<GetPointDTO>();
            _mockedPointService.Setup(obj => obj.GetAllPointsAsync(It.IsAny<long>())).ReturnsAsync(returnData).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);
            pointController.ControllerContext = _controllerContext;

            //Act
            var result = await pointController.Get();
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.GetAllPointsAsync(It.IsAny<long>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            Assert.True((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Data);
            Assert.NotNull((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Message);
            Assert.Equal((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Message, expectedMessage);
        }



        /// <summary>
        /// Delete Point Method Test when Invalid pointId
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePointTest_InvalidPointId()
        {
            //Arrange
            var payload = default(long);
            var returnData = (default(bool), string.Empty);
            const string expectedErrorMessage = "Invalid point id.";
            _mockedPointService.Setup(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(returnData).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);

            //Act
            var result = await pointController.Delete(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.Null((objectResult.Value as Response<string>)?.Data);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, expectedErrorMessage);
        }

        /// <summary>
        /// Delete Point Method Test when pointId is valid but other validation failed
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePointTest_ValidPointIdButOtherValidationFailed()
        {
            //Arrange    
            var payload = 1L;
            var returnData = (default(bool), "Some Error message");
            _mockedPointService.Setup(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(returnData).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);
            pointController.ControllerContext = _controllerContext;

            //Act
            var result = await pointController.Delete(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.Null((objectResult.Value as Response<string>)?.Data);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, returnData.Item2);
        }

        /// <summary>
        /// Delete Point Method Test Successful Deletion
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePointTest_SuccessfulDeletion()
        {
            //Arrange    
            var payload = 1L;
            var returnData = (true, string.Empty);
            const string expectedMessage = "Successfully deleted.";
            _mockedPointService.Setup(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(returnData).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);
            pointController.ControllerContext = _controllerContext;

            //Act
            var result = await pointController.Delete(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            Assert.True((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.Null((objectResult.Value as Response<string>)?.Data);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, expectedMessage);
        }

        /// <summary>
        /// Import Points from body Method Test when Null Payload
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportPointsFromBodyTest_NullPayload()
        {
            //Arrange
            var payload = default(IEnumerable<PointDTO>);
            const string expectedErrorMessage = "Invalid payload.";
            _mockedPointService.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>())).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);

            //Act
            var result = await pointController.Import(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>()), Times.Never());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.Null((objectResult.Value as Response<string>)?.Data);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, expectedErrorMessage);
        }


        /// <summary>
        /// Import Points from body Method Test when Not Null Payload but other validation failed
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportPointsFromBodyTest_NotNullPayloadButOtherValidationFailed()
        {
            //Arrange
            var payload = Enumerable.Empty<PointDTO>();
            var returnData = (default(IEnumerable<GetPointDTO>), "Some validation failed.");
            _mockedPointService.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>())).ReturnsAsync(returnData).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);
            pointController.ControllerContext = _controllerContext;


            //Act
            var result = await pointController.Import(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.Null((objectResult.Value as Response<string>)?.Data);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, returnData.Item2);
        }

        /// <summary>
        /// Import Points from body Method Test Success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportPointsFromBodyTest_Successful()
        {
            //Arrange
            var payload = new PointDTO[] { new PointDTO { X = 1, Y = 2 } };
            var returnData = (new GetPointDTO[] { new GetPointDTO { X = 1, Y = 2 } }, string.Empty);
            const string expectedMessage = "Successfully imported.";
            _mockedPointService.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>())).ReturnsAsync(returnData).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);
            pointController.ControllerContext = _controllerContext;


            //Act
            var result = await pointController.Import(payload);
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            Assert.True((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Data);
            Assert.True((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Data.Count() > 0);
            Assert.NotNull((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Message);
            Assert.Equal((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Message, expectedMessage);
        }

        /// <summary>
        /// Import Points from file Method Test when no file uploaded
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportPointsFromFileTest_NoFile()
        {
            //Arrange 
            const string expectedErrorMessage = "Invalid file.";
            _mockedPointService.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>())).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);

            //Act
            var result = await pointController.ImportFromCSV();
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>()), Times.Never());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.Null((objectResult.Value as Response<string>)?.Data);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, expectedErrorMessage);
        }

        /// <summary>
        /// Import Points from file Method Test when file with invalid data uploaded
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportPointsFromFileTest_InvalidFileContent()
        {
            //Arrange 
            var returnData = (default(IEnumerable<GetPointDTO>), "Some validation failed.");
            _mockedPointService.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>())).ReturnsAsync(returnData).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);
            _controllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var filecontent = Encoding.UTF8.GetBytes("some invalid content");
            var file = new FormFile(new MemoryStream(filecontent), 0, filecontent.Length, "formData", "dummy.csv");
            _controllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });
            pointController.ControllerContext = _controllerContext;


            //Act
            var result = await pointController.ImportFromCSV();
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            Assert.False((objectResult.Value as Response<string>)?.IsSuccess);
            Assert.Null((objectResult.Value as Response<string>)?.Data);
            Assert.NotNull((objectResult.Value as Response<string>)?.Message);
            Assert.Equal((objectResult.Value as Response<string>)?.Message, returnData.Item2);
        }

        /// <summary>
        /// Import Points from file Method Test success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportPointsFromFileTest_Success()
        {
            //Arrange 
            var returnData = (new GetPointDTO[] { new GetPointDTO { X = 1, Y = 2 } }, string.Empty);
            const string expectedMessage = "Successfully imported.";
            _mockedPointService.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>())).ReturnsAsync(returnData).Verifiable();

            var pointController = new PointsController(_mockedPointService.Object);
            _controllerContext.HttpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var filecontent = Encoding.UTF8.GetBytes("12,2\n3,4\n5,6");
            var file = new FormFile(new MemoryStream(filecontent), 0, filecontent.Length, "formData", "dummy.csv");
            _controllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });
            pointController.ControllerContext = _controllerContext;


            //Act
            var result = await pointController.ImportFromCSV();
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedPointService.Verify(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PointDTO>>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            Assert.True((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Data);
            Assert.True((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Data.Count() > 0);
            Assert.NotNull((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Message);
            Assert.Equal((objectResult.Value as Response<IEnumerable<GetPointDTO>>)?.Message, expectedMessage);
        }

    }
}

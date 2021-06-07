using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SquaresApp.API.Controllers.v1;
using SquaresApp.Application.IServices;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.API.Tests
{
    public class SquareControllerTests : SecuredControllerBase
    {
        private readonly Mock<ISquaresService> _mockedSquaresService;
        public SquareControllerTests()
        {
            _mockedSquaresService = new Mock<ISquaresService>();
        }


        /// <summary>
        /// Get Squares Method Test
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSquaresTest()
        {
            //Arrange    
            var returnData = new SquareDTO[] { };
            const string expectedMessage = "Successfully identified.";
            _mockedSquaresService.Setup(obj => obj.GetAllSquares(It.IsAny<long>())).ReturnsAsync(returnData).Verifiable();

            var squareController = new SquareController(_mockedSquaresService.Object);
            squareController.ControllerContext = _controllerContext;

            //Act
            var result = await squareController.Get();
            var objectResult = result as ObjectResult;


            //Assert    
            _mockedSquaresService.Verify(obj => obj.GetAllSquares(It.IsAny<long>()), Times.Once());
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            Assert.True((objectResult.Value as Response<IEnumerable<SquareDTO>>)?.IsSuccess);
            Assert.NotNull((objectResult.Value as Response<IEnumerable<SquareDTO>>)?.Data);
            Assert.NotNull((objectResult.Value as Response<IEnumerable<SquareDTO>>)?.Message);
            Assert.Equal((objectResult.Value as Response<IEnumerable<SquareDTO>>)?.Message, expectedMessage);
        }

    }
}

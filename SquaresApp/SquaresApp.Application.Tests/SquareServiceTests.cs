using Moq;
using SquaresApp.Application.Services;
using SquaresApp.Common.DTOs;
using SquaresApp.Data.IRepositories;
using SquaresApp.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.Application.Tests
{
    public class SquareServiceTests : ServiceBase
    {
        private readonly Mock<IPointRepository> _mockedPointRepository;
        private const long _userId = 1;
        private const int _zero = 0;
        public SquareServiceTests()
        {
            _mockedPointRepository = new Mock<IPointRepository>();
        }


        /// <summary>
        /// GetAllSquares Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllSquaresTest_Successful()
        {

            //Arrange      
            _mockedPointRepository.Setup(obj => obj.GetAllPointsAsync(It.IsAny<long>())).ReturnsAsync(Enumerable.Empty<Point>()).Verifiable();

            var pointService = new SquaresService(_mapper, _mockedPointRepository.Object);


            //Act
            var result = await pointService.GetAllSquares(_userId);


            //Assert   
            _mockedPointRepository.Verify(obj => obj.GetAllPointsAsync(It.IsAny<long>()), Times.Once());
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<SquareDTO>>(result);
            Assert.Equal(result.Count(), _zero);
        }

    }
}

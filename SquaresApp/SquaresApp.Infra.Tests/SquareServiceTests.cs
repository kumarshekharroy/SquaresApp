using Moq;
using SquaresApp.Common.DTOs;
using SquaresApp.Domain.IRepositories;
using SquaresApp.Domain.Models;
using SquaresApp.Infra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.Infra.Tests
{
    public class SquareServiceTests: ServiceBase
    {
        private readonly Mock<IPointRepository> _mockedPointrRepository;
        private const long _userId = 1;
        private const int _zero = 0;
        public SquareServiceTests()
        {
            _mockedPointrRepository = new Mock<IPointRepository>();
        }


        /// <summary>
        /// GetAllSquares Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllSquaresTest_Successful()
        {

            //Arrange      
            _mockedPointrRepository.Setup(obj => obj.GetAllPointsAsync(It.IsAny<long>())).ReturnsAsync(Enumerable.Empty<Point>()).Verifiable();

            var pointService = new SquaresService(_mapper, _mockedPointrRepository.Object);


            //Act
            var result = await pointService.GetAllSquares(_userId);


            //Assert   
            _mockedPointrRepository.Verify(obj => obj.GetAllPointsAsync(It.IsAny<long>()), Times.Once());
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<SquareDTO>>(result);
            Assert.Equal(result.Count(), _zero);
        }

    }
}

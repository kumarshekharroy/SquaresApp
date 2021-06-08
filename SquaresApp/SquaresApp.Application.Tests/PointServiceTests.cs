using Moq;
using SquaresApp.Application.Services;
using SquaresApp.Common.Constants;
using SquaresApp.Common.DTOs;
using SquaresApp.Data.IRepositories;
using SquaresApp.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.Application.Tests
{
    public class PointServiceTests : ServiceBase
    {
        private readonly Mock<IPointRepository> _mockedPointRepository;
        private const long _userId = 1;
        private const int _zero = 0;
        public PointServiceTests()
        {
            _mockedPointRepository = new Mock<IPointRepository>();
        } 

        /// <summary>
        /// GetAllPoints Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllPointsTest_Successful()
        {

            //Arrange      
            _mockedPointRepository.Setup(obj => obj.GetAllPointsAsync(It.IsAny<long>())).ReturnsAsync(Enumerable.Empty<Point>()).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointRepository.Object);


            //Act
            var result = await pointService.GetAllPointsAsync(_userId);


            //Assert   
            _mockedPointRepository.Verify(obj => obj.GetAllPointsAsync(It.IsAny<long>()), Times.Once());
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<GetPointDTO>>(result);
            Assert.Equal(result.Count(), _zero);
        }

        /// <summary>
        /// DeletePoint Test UnSuccessful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePointTest_UnSuccessful()
        {

            //Arrange      
            long pointId = 1;
            var expectedResult = (default(bool), "someError message.");
            _mockedPointRepository.Setup(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(expectedResult).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointRepository.Object);


            //Act
            var result = await pointService.DeletePointAsync(_userId, pointId);


            //Assert   
            _mockedPointRepository.Verify(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Once());
            Assert.False(result.isDeleted);
            Assert.NotEmpty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.Item2);
        }

        /// <summary>
        /// DeletePoint Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePointTest_Successful()
        {

            //Arrange      
            long pointId = 1;
            var expectedResult = (true, errorMessage: string.Empty);
            _mockedPointRepository.Setup(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(expectedResult).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointRepository.Object);


            //Act
            var result = await pointService.DeletePointAsync(_userId, pointId);


            //Assert   
            _mockedPointRepository.Verify(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Once());
            Assert.True(result.isDeleted);
            Assert.Empty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);
        }



        /// <summary>
        /// AddPoints Test For Empty List
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPointsTest_EmptyList()
        {

            //Arrange    
            var pointDTOs = Enumerable.Empty<PointDTO>();
            var expectedResult = (default(IEnumerable<GetPointDTO>), errorMessage: "No point supplied.");

            _mockedPointRepository.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<Point>>())).ReturnsAsync((default(IEnumerable<Point>), expectedResult.errorMessage)).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointRepository.Object);


            //Act
            var result = await pointService.AddAllPointsAsync(_userId, pointDTOs);


            //Assert   
            _mockedPointRepository.Verify(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<Point>>()), Times.Never());
            Assert.Null(result.getPointDTOs);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);

        }

        /// <summary>
        /// AddPoints Test For Some failing some validation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPointsTest_FailedSomeValidation()
        {

            //Arrange    
            var pointDTOs = new PointDTO[] { new PointDTO() { X = 1, Y = 2 } };
            var expectedResult = (default(IEnumerable<GetPointDTO>), errorMessage:"Some Other Error.");

            _mockedPointRepository.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<Point>>())).ReturnsAsync((default(IEnumerable<Point>), expectedResult.errorMessage)).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointRepository.Object);


            //Act
            var result = await pointService.AddAllPointsAsync(_userId, pointDTOs);


            //Assert   
            _mockedPointRepository.Verify(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<Point>>()), Times.Once());
            Assert.Null(result.getPointDTOs);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);

        }


        /// <summary>
        /// AddPoints Test Success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPointsTest_Successful()
        {

            //Arrange    
            var pointDTOs = new PointDTO[] { new PointDTO() { X = 1, Y = 2 } };
            var expectedResult = (new GetPointDTO[] { new GetPointDTO() { X = 1, Y = 2 } }, errorMessage: string.Empty);

            _mockedPointRepository.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<Point>>())).ReturnsAsync((new Point[] { new Point() { X = 1, Y = 2 } }, expectedResult.errorMessage)).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointRepository.Object);


            //Act
            var result = await pointService.AddAllPointsAsync(_userId, pointDTOs);


            //Assert   
            _mockedPointRepository.Verify(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<Point>>()), Times.Once());
            Assert.NotNull(result.getPointDTOs);
            Assert.True(result.getPointDTOs.Any());
            Assert.Empty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.errorMessage);

        }

    }
}

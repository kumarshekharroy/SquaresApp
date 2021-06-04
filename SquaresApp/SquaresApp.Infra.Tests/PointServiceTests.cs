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
    public class PointServiceTests : ServiceBase
    {
        private readonly Mock<IPointRepository> _mockedPointrRepository;
        private const long _userId = 1;
        private const int _zero = 0;
        public PointServiceTests()
        {
            _mockedPointrRepository = new Mock<IPointRepository>();
        }


        /// <summary>
        /// AddPoint Test For Some failing some validation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPointTest_FailedSomeValidation()
        {

            //Arrange    
            var pointDTO = new PointDTO() { X = 1, Y = 2 };
            var expectedResult = (default(GetPointDTO), "someError message.");

            _mockedPointrRepository.Setup(obj => obj.AddPointAsync(It.IsAny<Point>())).ReturnsAsync((default(Point), expectedResult.Item2)).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointrRepository.Object);


            //Act
            var result = await pointService.AddPointAsync(_userId, pointDTO);


            //Assert   
            _mockedPointrRepository.Verify(obj => obj.AddPointAsync(It.IsAny<Point>()), Times.Once());
            Assert.Null(result.getPointDTO);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.Item2);

        }

        /// <summary>
        /// AddPoint Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPointTest_Successful()
        {

            //Arrange    
            var pointDTO = new PointDTO() { X = 1, Y = 2 };
            var expectedResult = (new GetPointDTO { Id = 1, X = pointDTO.X, Y = pointDTO.Y }, string.Empty);

            _mockedPointrRepository.Setup(obj => obj.AddPointAsync(It.IsAny<Point>())).ReturnsAsync((new Point { UserId = _userId, X = pointDTO.X, Y = pointDTO.Y }, expectedResult.Item2)).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointrRepository.Object);


            //Act
            var result = await pointService.AddPointAsync(_userId, pointDTO);


            //Assert   
            _mockedPointrRepository.Verify(obj => obj.AddPointAsync(It.IsAny<Point>()), Times.Once());
            Assert.NotNull(result.getPointDTO);
            Assert.Equal(result.getPointDTO.X, expectedResult.Item1.X);
            Assert.Equal(result.getPointDTO.Y, expectedResult.Item1.Y);
            Assert.Empty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.Item2);

        }

        /// <summary>
        /// GetAllPoints Test Successful
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllPointsTest_Successful()
        {

            //Arrange      
            _mockedPointrRepository.Setup(obj => obj.GetAllPointsAsync(It.IsAny<long>())).ReturnsAsync(Enumerable.Empty<Point>()).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointrRepository.Object);


            //Act
            var result = await pointService.GetAllPointsAsync(_userId);


            //Assert   
            _mockedPointrRepository.Verify(obj => obj.GetAllPointsAsync(It.IsAny<long>()), Times.Once());
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
            _mockedPointrRepository.Setup(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(expectedResult).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointrRepository.Object);


            //Act
            var result = await pointService.DeletePointAsync(_userId, pointId);


            //Assert   
            _mockedPointrRepository.Verify(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Once());
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
            var expectedResult = (true, string.Empty);
            _mockedPointrRepository.Setup(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(expectedResult).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointrRepository.Object);


            //Act
            var result = await pointService.DeletePointAsync(_userId, pointId);


            //Assert   
            _mockedPointrRepository.Verify(obj => obj.DeletePointAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Once());
            Assert.True(result.isDeleted);
            Assert.Empty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.Item2);
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
            var expectedResult = (default(IEnumerable<GetPointDTO>), "No point supplied.");

            _mockedPointrRepository.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<Point>>())).ReturnsAsync((default(IEnumerable<Point>), expectedResult.Item2)).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointrRepository.Object);


            //Act
            var result = await pointService.AddAllPointsAsync(_userId, pointDTOs);


            //Assert   
            _mockedPointrRepository.Verify(obj => obj.AddPointAsync(It.IsAny<Point>()), Times.Never());
            Assert.Null(result.getPointDTOs);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.Item2);

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
            var expectedResult = (default(IEnumerable<GetPointDTO>), "Some Other Error.");

            _mockedPointrRepository.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<Point>>())).ReturnsAsync((default(IEnumerable<Point>), expectedResult.Item2)).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointrRepository.Object);


            //Act
            var result = await pointService.AddAllPointsAsync(_userId, pointDTOs);


            //Assert   
            _mockedPointrRepository.Verify(obj => obj.AddPointAsync(It.IsAny<Point>()), Times.Never());
            Assert.Null(result.getPointDTOs);
            Assert.NotNull(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.Item2);

        }


        /// <summary>
        /// AddPoints Test Success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddPointsTest_SuccessFful()
        {

            //Arrange    
            var pointDTOs = new PointDTO[] { new PointDTO() { X = 1, Y = 2 } };
            var expectedResult = (new GetPointDTO[] { new GetPointDTO() { X = 1, Y = 2 } }, string.Empty);

            _mockedPointrRepository.Setup(obj => obj.AddAllPointsAsync(It.IsAny<long>(), It.IsAny<IEnumerable<Point>>())).ReturnsAsync((new Point[] { new Point() { X = 1, Y = 2 } }, expectedResult.Item2)).Verifiable();

            var pointService = new PointService(_mapper, _mockedPointrRepository.Object);


            //Act
            var result = await pointService.AddAllPointsAsync(_userId, pointDTOs);


            //Assert   
            _mockedPointrRepository.Verify(obj => obj.AddPointAsync(It.IsAny<Point>()), Times.Never());
            Assert.NotNull(result.getPointDTOs);
            Assert.True(result.getPointDTOs.Count()>0);
            Assert.Empty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedResult.Item2);

        }

    }
}

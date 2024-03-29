using SquaresApp.Data.Repositories;
using SquaresApp.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SquaresApp.Data.Tests
{
    public class PointRepositoryTests : RepositoryBase
    {
        

        /// <summary>
        /// DeletePoint Test For Invalidpoint Id 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePoint_InvalidPointId()
        {

            //Arrange   
            var pointId = 10L;
            var expectedMessage = "Point doesn't exist.";

            var pointRepository = new PointRepository(_squaresAppDBContext);


            //Act
            var result = await pointRepository.DeletePointAsync(userId, pointId);


            //Assert    
            Assert.False(result.isDeleted);
            Assert.NotEmpty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedMessage);
        }

        /// <summary>
        /// DeletePoint Test Success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeletePoint_Success()
        {

            //Arrange   
            var pointId = 1;
            var expectedMessage = string.Empty;

            var pointRepository = new PointRepository(_squaresAppDBContext);


            //Act
            var result = await pointRepository.DeletePointAsync(userId, pointId);


            //Assert    
            Assert.True(result.isDeleted);
            Assert.Empty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedMessage);
        }


        /// <summary>
        /// GetAllPoints Test Success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllPoints_Success()
        {

            //Arrange     

            var pointRepository = new PointRepository(_squaresAppDBContext);


            //Act
            var result = await pointRepository.GetAllPointsAsync(userId);


            //Assert    
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Point>>(result);
            Assert.True(result.Any());
        }

        /// <summary>
        /// AddAllPoint Test For Duplicate 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAllPoint_AlreadyExist()
        {

            //Arrange   
            var points = new Point[] { new Point() { X = -1, Y = -1, UserId = userId } };
            var expectedMessage = "Points already exist.";

            var pointRepository = new PointRepository(_squaresAppDBContext);


            //Act
            var result = await pointRepository.AddAllPointsAsync(userId, points);


            //Assert    
            Assert.Null(result.points);
            Assert.NotEmpty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedMessage);
        }

        /// <summary>
        /// AddAllPoint Test Success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAllPoint_Successful()
        {

            //Arrange   
            var points = new Point[] { new Point() { X = -16, Y = -17, UserId = userId } };
            var expectedMessage = string.Empty;

            var pointRepository = new PointRepository(_squaresAppDBContext);


            //Act
            var result = await pointRepository.AddAllPointsAsync(userId, points);


            //Assert    
            Assert.NotNull(result.points);
            Assert.True(result.points.Any());
            Assert.Empty(result.errorMessage);
            Assert.Equal(result.errorMessage, expectedMessage);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using SquaresApp.Common.Constants;
using SquaresApp.Data.Context;
using SquaresApp.Data.EqualityComparers;
using SquaresApp.Data.IRepositories;
using SquaresApp.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquaresApp.Data.Repositories
{
    public class PointRepository : IPointRepository
    {
        private readonly SquaresAppDBContext _squaresAppDBContext;

        public PointRepository(SquaresAppDBContext squaresAppDBContext)
        {
            _squaresAppDBContext = squaresAppDBContext;
        }


        /// <summary>
        /// add all points
        /// </summary>  
        /// <param name="userId"></param>  
        /// <param name="points"></param>  
        /// <returns></returns>
        public async Task<(IEnumerable<Point> points, string errorMessage)> AddAllPointsAsync(long userId, IEnumerable<Point> points)
        {
            var existingPoints = await _squaresAppDBContext.Points.Where(rec => rec.UserId == userId).ToArrayAsync();

            var pointsToBeAdded = points.Distinct(new PointEqualityComparer()).Except(existingPoints, new PointEqualityComparer());

            if (!pointsToBeAdded.Any())
            {
                return (points: default, errorMessage: "Points already exist.");
            }

            await _squaresAppDBContext.Points.AddRangeAsync(pointsToBeAdded);

            if (await _squaresAppDBContext.SaveChangesAsync() == 0)
            {
                return (points: default, errorMessage: ConstantValues.UnexpectedErrorMessage);
            }

            return (points: pointsToBeAdded, errorMessage: string.Empty);

        }
         
        /// <summary>
        /// delete a point from existing list of points
        /// </summary>
        /// <param name="userId"></param> 
        /// <param name="pointId"></param> 
        /// <returns></returns>
        public async Task<(bool isDeleted, string errorMessage)> DeletePointAsync(long userId, long pointId)
        {

            var point = await _squaresAppDBContext.Points.FirstOrDefaultAsync(rec => rec.UserId == userId && rec.Id == pointId);

            if (point is null)
            {
                return (isDeleted: default, errorMessage: "Point doesn't exist.");
            }

            _squaresAppDBContext.Points.Remove(point);

            if (await _squaresAppDBContext.SaveChangesAsync() == 0)
            {
                return (isDeleted: default, errorMessage: ConstantValues.UnexpectedErrorMessage);
            }

            return (isDeleted: true, errorMessage: string.Empty);
        }

        /// <summary>
        /// get all points
        /// </summary>
        /// <param name="userId"></param>  
        /// <returns></returns>
        public async Task<IEnumerable<Point>> GetAllPointsAsync(long userId)
        {
            return await _squaresAppDBContext.Points.Where(rec => rec.UserId == userId).ToArrayAsync();
        }

    }
}

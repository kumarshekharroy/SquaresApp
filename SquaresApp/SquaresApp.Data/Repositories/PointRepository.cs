using Microsoft.EntityFrameworkCore;
using SquaresApp.Common.Constants;
using SquaresApp.Data.Context;
using SquaresApp.Domain.EqualityComparers;
using SquaresApp.Domain.IRepositories;
using SquaresApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var pointsTobeAdded = points.Distinct(new PointEqualityComparer()).Except(existingPoints, new PointEqualityComparer());

            if (pointsTobeAdded.Count() == 0)
            {
                return (points: default, errorMessage: "Points already exist.");
            }

            await _squaresAppDBContext.Points.AddRangeAsync(pointsTobeAdded);

            if (await _squaresAppDBContext.SaveChangesAsync() == 0)
            {
                return (points: default, errorMessage: ConstantValues.UnexpectedErrorMessage);
            }

            return (points: pointsTobeAdded, errorMessage: string.Empty);

        }

        /// <summary>
        /// add a new point in existing list of points
        /// </summary> 
        /// <param name="point"></param> 
        /// <returns></returns>
        public async Task<(Point point, string errorMessage)> AddPointAsync(Point point)
        {

            var pointExists = await _squaresAppDBContext.Points.AnyAsync(rec => rec.X == point.X && rec.Y == point.Y);

            if (pointExists)
            {
                return (point: default, errorMessage: "Point already exists.");
            }

            await _squaresAppDBContext.Points.AddAsync(point);

            if (await _squaresAppDBContext.SaveChangesAsync() == 0)
            {
                return (point: default, errorMessage: ConstantValues.UnexpectedErrorMessage);
            }

            return (point: point, errorMessage: string.Empty);

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

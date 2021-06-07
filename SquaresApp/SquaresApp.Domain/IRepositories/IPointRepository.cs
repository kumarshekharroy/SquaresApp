﻿using SquaresApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SquaresApp.Domain.IRepositories
{
    public interface IPointRepository
    {
        /// <summary>
        /// add a new point in existing list of points
        /// </summary> 
        /// <param name="point"></param> 
        /// <returns></returns>
        Task<(Point point, string errorMessage)> AddPointAsync(Point point);


        /// <summary>
        /// delete a point from existing list of points
        /// </summary>
        /// <param name="userId"></param> 
        /// <param name="pointId"></param> 
        /// <returns></returns>
        Task<(bool isDeleted, string errorMessage)> DeletePointAsync(long userId, long pointId);


        /// <summary>
        /// get all points
        /// </summary>
        /// <param name="userId"></param>  
        /// <returns></returns>
        Task<IEnumerable<Point>> GetAllPointsAsync(long userId);


        /// <summary>
        /// add all points
        /// </summary>  
        /// <param name="points"></param>  
        /// <param name="userId"></param>  
        /// <returns></returns>
        Task<(IEnumerable<Point> points, string errorMessage)> AddAllPointsAsync(long userId, IEnumerable<Point> points);


    }
}

using SquaresApp.Common.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SquaresApp.Application.IServices
{
    public interface IPointService
    {
        /// <summary>
        /// add a new point in existing list of points
        /// </summary>
        /// <param name="userID"></param> 
        /// <param name="pointDTO"></param> 
        /// <returns></returns>
        Task<(GetPointDTO getPointDTO, string errorMessage)> AddPointAsync(long userId, PointDTO pointDTO);


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
        /// <param name="userID"></param>  
        /// <returns></returns>
        Task<IEnumerable<GetPointDTO>> GetAllPointsAsync(long userId);


        /// <summary>
        /// add all points
        /// </summary>
        /// <param name="userID"></param>  
        /// <param name="pointDTOs"></param>  
        /// <returns></returns>
        Task<(IEnumerable<GetPointDTO> getPointDTOs, string errorMessage)> AddAllPointsAsync(long userId, IEnumerable<PointDTO> pointDTOs);


    }
}

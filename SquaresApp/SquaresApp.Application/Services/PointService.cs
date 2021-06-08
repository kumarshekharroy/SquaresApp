using AutoMapper;
using SquaresApp.Application.IServices;
using SquaresApp.Common.Constants;
using SquaresApp.Common.DTOs;
using SquaresApp.Data.IRepositories;
using SquaresApp.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquaresApp.Application.Services
{
    public class PointService : IPointService
    {

        private readonly IPointRepository _pointRepository;
        private readonly IMapper _mapper;
        public PointService(IMapper mapper, IPointRepository pointRepository)
        {
            _mapper = mapper;
            _pointRepository = pointRepository;
        }

        /// <summary>
        /// add all points
        /// </summary>
        /// <param name="userId"></param>  
        /// <param name="pointDTOs"></param>  
        /// <returns></returns>
        public async Task<(IEnumerable<GetPointDTO> getPointDTOs, string errorMessage)> AddAllPointsAsync(long userId, IEnumerable<PointDTO> pointDTOs)
        {
            if (!pointDTOs.Any())
            {
                return (getPointDTOs: default, errorMessage: "No point supplied.");
            }

            var points = _mapper.Map<IEnumerable<Point>>(pointDTOs, opt =>
            {
                opt.Items[ConstantValues.UserId] = userId;
            });

            var result = await _pointRepository.AddAllPointsAsync(userId, points);

            if (!string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return (getPointDTOs: default, result.errorMessage);
            }

            var getPointDTOs = _mapper.Map<IEnumerable<GetPointDTO>>(result.points);

            return (getPointDTOs: getPointDTOs, errorMessage: result.errorMessage);

        } 

        /// <summary>
        /// delete a point from existing list of points
        /// </summary>
        /// <param name="userId"></param> 
        /// <param name="pointId"></param> 
        /// <returns></returns>
        public async Task<(bool isDeleted, string errorMessage)> DeletePointAsync(long userId, long pointId)
        {
            return await _pointRepository.DeletePointAsync(userId, pointId);
        }

        /// <summary>
        /// get all points
        /// </summary>
        /// <param name="userId"></param>  
        /// <returns></returns>
        public async Task<IEnumerable<GetPointDTO>> GetAllPointsAsync(long userId)
        {
            var points = await _pointRepository.GetAllPointsAsync(userId);

            return _mapper.Map<IEnumerable<GetPointDTO>>(points);
        }

    }
}

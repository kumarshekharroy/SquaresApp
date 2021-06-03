using AutoMapper;
using SquaresApp.Common.Constants;
using SquaresApp.Common.DTOs;
using SquaresApp.Domain.IRepositories;
using SquaresApp.Domain.Models;
using SquaresApp.Infra.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Infra.Services
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
            var points = _mapper.Map<IEnumerable<Point>>(pointDTOs, opt =>
            {
                opt.Items[ConstantValues.UserId] = userId;
            });

            var result = await _pointRepository.AddAllPointsAsync(userId, points);

            if (!string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return (getPointDTOs: default, errorMessage: result.errorMessage);
            }

            var getPointDTOs = _mapper.Map<IEnumerable<GetPointDTO>>(result.points);

            return (getPointDTOs: getPointDTOs, errorMessage: result.errorMessage);

        }

        /// <summary>
        /// add a new point in existing list of points
        /// </summary>
        /// <param name="userId"></param> 
        /// <param name="pointDTO"></param> 
        /// <returns></returns>
        public async Task<(GetPointDTO getPointDTO, string errorMessage)> AddPointAsync(long userId, PointDTO pointDTO)
        {
            var point = _mapper.Map<Point>(pointDTO, opt =>
            {
                opt.Items[ConstantValues.UserId] = userId;
            });

            var result = await _pointRepository.AddPointAsync(point);

            if (!string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return (getPointDTO: default, errorMessage: result.errorMessage);
            }

            var getPointDTO = _mapper.Map<GetPointDTO>(result.point);

            return (getPointDTO: getPointDTO, errorMessage: result.errorMessage);

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

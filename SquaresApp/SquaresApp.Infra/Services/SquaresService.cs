using AutoMapper;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.EqualityComparers;
using SquaresApp.Common.Helpers;
using SquaresApp.Domain.EqualityComparers;
using SquaresApp.Domain.IRepositories;
using SquaresApp.Domain.Models;
using SquaresApp.Infra.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Infra.Services
{
    public class SquaresService : ISquaresService
    {

        private readonly IPointRepository _pointRepository;
        private readonly IMapper _mapper;
        public SquaresService(IMapper mapper, IPointRepository pointRepository)
        {
            _mapper = mapper;
            _pointRepository = pointRepository;
        }

        /// <summary>
        /// returns coordinate of all vertices of squares from existing list of points
        /// </summary>
        /// <param name="userId"></param>   
        /// <returns></returns>
        public async Task<IEnumerable<SquareDTO>> GetAllSquares(long userId)
        {
            var points = _mapper.Map<IEnumerable<GetPointDTO>>(await _pointRepository.GetAllPointsAsync(userId)).ToHashSet(new GetPointDTOEqualityComparer());

            var squaresFound = new HashSet<SquareDTO>(new SquareDTOEqualityComparer());

            foreach (var a in points)
            {
                foreach (var c in points)
                { 
                    if (a.X == c.X && a.Y == c.Y)
                    {
                        continue;
                    }

                    var remainingPoints = PointHelper.GetRemainingPoints(a, c);

                    if (points.TryGetValue(remainingPoints.b, out var b) && points.TryGetValue(remainingPoints.d, out var d))
                    {
                        var allFourVerticesSortedWRTAngleWithXAxis = new GetPointDTO[] { a, b, c, d }.OrderBy(x => Math.Atan2(x.X, x.Y)).ToArray();

                        var squareDTO = new SquareDTO { A = allFourVerticesSortedWRTAngleWithXAxis[0], B = allFourVerticesSortedWRTAngleWithXAxis[1], C = allFourVerticesSortedWRTAngleWithXAxis[2], D = allFourVerticesSortedWRTAngleWithXAxis[3] };

                        if (squaresFound.Contains(squareDTO))
                        {
                            continue;
                        }

                        squaresFound.Add(squareDTO);
                    }
                }

            }
            return squaresFound;
        }

    }
}

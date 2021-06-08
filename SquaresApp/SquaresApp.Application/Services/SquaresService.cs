using AutoMapper;
using SquaresApp.Application.IServices;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.EqualityComparers;
using SquaresApp.Common.Helpers;
using SquaresApp.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquaresApp.Application.Services
{
    public class SquaresService : ISquaresService
    {

        private readonly IPointRepository _pointRepository;
        private readonly IMapper _mapper;

        private readonly GetPointDTOEqualityComparer _getPointDTOEqualityComparer = new GetPointDTOEqualityComparer();
        private readonly SquareDTOEqualityComparer _squareDTOEqualityComparer = new SquareDTOEqualityComparer();

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
            var points = _mapper.Map<GetPointDTO[]>(await _pointRepository.GetAllPointsAsync(userId));

            var pointsHashset = points.ToHashSet(_getPointDTOEqualityComparer); //converted to hashset to ensure O(1) lookup time.

            var squaresFound = new HashSet<SquareDTO>(_squareDTOEqualityComparer);

            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++) // started from i+1 cuz line connecting points A and B will be same as Line connecting points B and A. i.e. Line AB == Line BA.
                {
                    var a = points[i];
                    var c = points[j];

                    var remainingDiagonalPoints = PointHelper.GetRemainingPoints(a, c); // find coordinates of the expected 2nd diagonal of the square.

                    if (pointsHashset.TryGetValue(remainingDiagonalPoints.b, out var b) && pointsHashset.TryGetValue(remainingDiagonalPoints.d, out var d))
                    {
                        var allFourVerticesSortedWRTAngleWithOrigin = new GetPointDTO[] { a, b, c, d }.OrderBy(x => Math.Atan2(x.X, x.Y)).ToArray(); // sort vertices of the square (rotate WRT Origin) to filter out overlapping squares

                        var squareDTO = new SquareDTO { A = allFourVerticesSortedWRTAngleWithOrigin[0], B = allFourVerticesSortedWRTAngleWithOrigin[1], C = allFourVerticesSortedWRTAngleWithOrigin[2], D = allFourVerticesSortedWRTAngleWithOrigin[3] };

                        squaresFound.Add(squareDTO); // Hashset will take care of duplicates.
                    }
                }

            }
            return squaresFound;
        }

    }
}

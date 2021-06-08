using SquaresApp.Common.DTOs;
using System.Collections.Generic;

namespace SquaresApp.Common.EqualityComparers
{

    public sealed class SquareDTOEqualityComparer : IEqualityComparer<SquareDTO>
    {
        public bool Equals(SquareDTO one, SquareDTO other)
        {
            return one?.A?.X == other?.A?.X && one?.A?.Y == other?.A?.Y && one?.C?.X == other?.C?.X && one?.C?.Y == other?.C?.Y;
        }

        public int GetHashCode(SquareDTO obj)
        {
            return obj.GetHashCode();
        }
    }
}

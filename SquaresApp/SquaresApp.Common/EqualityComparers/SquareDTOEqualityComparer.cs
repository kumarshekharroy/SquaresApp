using SquaresApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Common.EqualityComparers
{

    public sealed class SquareDTOEqualityComparer : IEqualityComparer<SquareDTO>
    {
        public bool Equals(SquareDTO one, SquareDTO other)
        {
            if (one == null)
            {
                return other == null;
            }
            else if (other == null)
            {
                return false;
            }
            else
            {
                return one.A.X == other.A.X && one.A.Y == one.A.Y && one.C.X == other.C.X && one.C.Y == one.C.Y;
            }
        }

        public int GetHashCode(SquareDTO obj)
        {
            return obj.GetHashCode();
        }
    }
}

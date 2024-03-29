﻿using SquaresApp.Common.DTOs;
using System.Collections.Generic;

namespace SquaresApp.Common.EqualityComparers
{

    public sealed class GetPointDTOEqualityComparer : IEqualityComparer<GetPointDTO>
    {
        public bool Equals(GetPointDTO a, GetPointDTO b)
        {
           return a?.X == b?.X && a?.Y == b?.Y;
        }

        public int GetHashCode(GetPointDTO obj)
        {
            return obj.X.GetHashCode() ^ obj.Y.GetHashCode();
        }
    }
}

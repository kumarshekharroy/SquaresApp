﻿using SquaresApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Common.EqualityComparers
{
     
    public sealed class GetPointDTOEqualityComparer : IEqualityComparer<GetPointDTO>
    {
        public bool Equals(GetPointDTO a, GetPointDTO b)
        {
            if (a == null)
            {
                return b == null;
            }
            else if (b == null)
            {
                return false;
            }
            else
            {
                return a.X == b.X && a.Y == b.Y;
            }
        }

        public int GetHashCode(GetPointDTO obj)
        {
            return $"{obj.X}{obj.Y}".GetHashCode();
        }
    }
}
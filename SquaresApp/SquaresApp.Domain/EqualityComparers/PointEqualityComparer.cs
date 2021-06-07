﻿using SquaresApp.Domain.Models;
using System.Collections.Generic;

namespace SquaresApp.Domain.EqualityComparers
{

    public sealed class PointEqualityComparer : IEqualityComparer<Point>
    {
        public bool Equals(Point a, Point b)
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

        public int GetHashCode(Point obj)
        {
            return $"{obj.X}{obj.Y}".GetHashCode();
        }
    }
}

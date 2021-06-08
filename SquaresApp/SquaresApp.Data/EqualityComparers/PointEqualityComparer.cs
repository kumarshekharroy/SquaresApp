using SquaresApp.Data.Models;
using System.Collections.Generic;

namespace SquaresApp.Data.EqualityComparers
{

    public sealed class PointEqualityComparer : IEqualityComparer<Point>
    {
        public bool Equals(Point a, Point b)
        {
            return a?.X == b?.X && a?.Y == b?.Y;
        }

        public int GetHashCode(Point obj)
        {
            return $"{obj.X}{obj.Y}".GetHashCode();
        }
    }
}

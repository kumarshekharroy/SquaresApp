using SquaresApp.Common.DTOs;

namespace SquaresApp.Common.Helpers
{
    public static class PointHelper
    {
        public static (GetPointDTO b, GetPointDTO d) GetRemainingPoints(GetPointDTO a, GetPointDTO c)
        {
            //center point
            var midX = (a.X + c.X) / 2;
            var midY = (a.Y + c.Y) / 2;

            //half diagonal
            var diaX = (a.X - c.X) / 2;
            var diaY = (a.Y - c.Y) / 2;


            var bX = (midX - diaY);
            var bY = (midY + diaX);
            var b = new GetPointDTO { X = bX, Y = bY };

            var dX = (midX + diaY);
            var dY = (midY - diaX);
            var d = new GetPointDTO { X = dX, Y = dY };

            return (b, d);
        }
    }
}

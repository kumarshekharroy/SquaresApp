using SquaresApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Common.Helpers
{
    public class PointHelper
    { 
        public static (GetPointDTO b, GetPointDTO d) GetRemainingPoints(GetPointDTO a, GetPointDTO c)
        { 
            //center point
            var midX = (a.X + c.X) / 2M;
            var midY = (a.Y + c.Y) / 2M;

            //half diagonal
            var diaX = (a.X - c.X) / 2M;
            var diaY = (a.Y - c.Y) / 2M;


            int bX = (int)(midX - diaY);
            int bY = (int)(midY + diaX);
            var b = new GetPointDTO { X = bX, Y = bY };

            int dX = (int)(midX + diaY);
            int dY = (int)(midY - diaX);
            var d = new GetPointDTO { X = dX, Y = dY };
             
            return (b, d);
        }
    }
}

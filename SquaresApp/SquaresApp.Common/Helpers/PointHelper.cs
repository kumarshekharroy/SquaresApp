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

        //public static (GetPointDTO b, GetPointDTO d) GetRemainingPoints(GetPointDTO a, GetPointDTO c)
        //{

        //    int midX = (a.X + c.X) / 2;
        //    int midY = (a.Y + c.Y) / 2;

        //    int Ax = a.X - midX;
        //    int Ay = a.Y - midY;
        //    int bX = midX - Ay;
        //    int bY = midY + Ax;
        //    var b = new GetPointDTO { X = bX, Y = bY };

        //    int cX = (c.X - midX);
        //    int cY = (c.Y - midY);
        //    int dX = midX - cY;
        //    int dY = midY + cX;
        //    var d = new GetPointDTO { X = dX, Y = dY };
        //    return (b, d);
        //}


  //x1 = ?  ;  y1 = ? ;    // First diagonal point
  //x2 = ?  ;  y2 = ? ;    // Second diagonal point

  //xc = (x1 + x2)/2  ;  yc = (y1 + y2)/2  ;    // Center point
  //xd = (x1 - x2)/2  ;  yd = (y1 - y2)/2  ;    // Half-diagonal

  //x3 = xc - yd  ;  y3 = yc + xd;    // Third corner
  //x4 = xc + yd  ;  y4 = yc - xd;    // Fourth corner

        public static (GetPointDTO b, GetPointDTO d) GetRemainingPoints(GetPointDTO a, GetPointDTO c)
        { 
            //center point
            int midX = (a.X + c.X) / 2;
            int midY = (a.Y + c.Y) / 2;

            //half diagonal
            int diaX = (a.X - c.X) / 2;
            int diaY = (a.Y - c.Y) / 2;


            int bX = midX - diaY;
            int bY = midY + diaX;
            var b = new GetPointDTO { X = bX, Y = bY };

            int dX = midX + diaY;
            int dY = midY - diaX;
            var d = new GetPointDTO { X = dX, Y = dY };
             
            return (b, d);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Common.DTOs
{
    public class SquareDTO
    {
        public GetPointDTO A { get; set; }
        public GetPointDTO B { get; set; }
        public GetPointDTO C { get; set; }
        public GetPointDTO D { get; set; }

        public override int GetHashCode()
        {
            return A?.GetHashCode() ^ B?.GetHashCode()^C?.GetHashCode() ^ D?.GetHashCode()??0;
        }
    }
}

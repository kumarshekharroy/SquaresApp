using SquaresApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Infra.IServices
{
    public interface ISquaresService
    {
        /// <summary>
        /// returns coordinate of all vertices of squares from existing list of points
        /// </summary>
        /// <param name="userId"></param>   
        /// <returns></returns>
        Task<IEnumerable<SquareDTO>> GetAllSquares(long userId);
    }
}

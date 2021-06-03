using System;
using System.Collections.Generic;
using System.Text;

namespace SquaresApp.Common.DTOs
{
    /// <summary>
    /// Point's detail DTO
    /// </summary> 
    public class GetPointDTO : PointDTO
    {
        /// <summary>
        /// Id of the Point
        /// </summary>
        /// <example>7</example>
        public long Id { get; set; }
    }
}

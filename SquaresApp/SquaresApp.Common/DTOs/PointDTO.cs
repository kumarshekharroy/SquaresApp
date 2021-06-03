using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SquaresApp.Common.DTOs
{
    /// <summary>
    /// Point's detail DTO
    /// </summary> 
    public class PointDTO
    {

        /// <summary>
        /// X coordinate of the Point
        /// </summary>
        /// <example>-1</example> 
        public int X { get; set; }

        /// <summary>
        /// Y coordinate of the Point
        /// </summary>
        /// <example>1</example> 
        public int Y { get; set; }


        public int GetHashCode() => X ^ Y; 
         
    }
}

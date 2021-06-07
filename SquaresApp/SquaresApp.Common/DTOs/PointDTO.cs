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
        public float X { get; set; }

        /// <summary>
        /// Y coordinate of the Point
        /// </summary>
        /// <example>1</example> 
        public float Y { get; set; }


        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

    }
}

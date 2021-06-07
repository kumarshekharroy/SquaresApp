namespace SquaresApp.Domain.Models
{
    public class Point
    {
        public long Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
    }
}

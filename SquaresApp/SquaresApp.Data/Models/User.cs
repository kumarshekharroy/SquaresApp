using System.Collections.Generic;

namespace SquaresApp.Data.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Point> Points { get; set; }
    }
}

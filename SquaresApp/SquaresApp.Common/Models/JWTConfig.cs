using System;
using System.Collections.Generic;
using System.Text;

namespace SquaresApp.Common.Models
{
    public class JWTConfig
    {
        public string Secret { get; set; }
        public int ValidityInMinute { get; set; } = 60;
    }
}

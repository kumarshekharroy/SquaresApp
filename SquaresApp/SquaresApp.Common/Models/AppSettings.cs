using System;
using System.Collections.Generic;
using System.Text;

namespace SquaresApp.Common.Models
{
    public class AppSettings
    {
        public JWTConfig JWTConfig { get; set; }
        public CacheConfig CacheConfig { get; set; }
    }
}

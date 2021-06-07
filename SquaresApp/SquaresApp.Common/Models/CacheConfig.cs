namespace SquaresApp.Common.Models
{
    public class CacheConfig
    {
        public int SlidingExpirationTimeInMin { get; set; }
        public int AbsoluteExpirationTimeInMin { get; set; }
        public RedisConfig RedisConfig { get; set; }
    }
}

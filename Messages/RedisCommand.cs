
namespace Messages
{
    public class RedisCommand : Command
    {
        public int DatabaseId { get; set; }
        public override CacheEnum Cache { get; } = CacheEnum.Redis;
    }
}

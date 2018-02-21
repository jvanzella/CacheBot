using System.Runtime.Serialization;

namespace Messages
{
    [KnownType(typeof(RedisCommand))]
    public abstract class Command
    {
        public Environment Environment { get; set; } 
        public CommandType CommandType { get; set; }

        public abstract CacheEnum Cache { get; }
    }
}

using System;

namespace RedisClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RedisCacheService(12);
            client.Clear();
        }
    }
}

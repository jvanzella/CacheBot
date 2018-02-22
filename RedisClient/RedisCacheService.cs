using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq;
using System.Configuration;

namespace RedisClient
{
    public class RedisCacheService : ICacheService
    {
        private readonly int _DBInstance;
        private static string _connection = ConfigurationManager.AppSettings["RedisConnection"];

        public RedisCacheService(int DBInstance)
        {
            _DBInstance = DBInstance;
        }

        public Task<bool> Exists(string key) => Get(instance => instance.KeyExistsAsync(key));

        public Task<string> Get(string key) => Get(async instance => await instance.StringGetAsync(key).ContinueWith(x => x.ToString()));
        
        public Task Remove(string key) => Do(instance => instance.GetDatabase(_DBInstance).KeyDeleteAsync(key));
        

        public async Task Clear()
        {
            await Do(async instance =>
            {

                var endpoints = instance.GetEndPoints(true);
                var tasks = endpoints.Select(x => instance.GetServer(x).FlushDatabaseAsync(_DBInstance));
                await Task.WhenAll(tasks);
            });
        }

        private async Task<RedisCacheService> Do(Func<IConnectionMultiplexer, Task> action)
        {
            using (var instance = ConnectionMultiplexer.Connect(_connection))
            {
                await action(instance);
            }

            return this;
        }

        private async Task<T> Get<T>(Func<IDatabase, Task<T>> fn)
        {
            using (var instance = ConnectionMultiplexer.Connect(_connection))
            {
                return await fn(instance.GetDatabase(_DBInstance));
            }
        }
    }
}

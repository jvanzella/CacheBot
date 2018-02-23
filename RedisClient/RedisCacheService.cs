using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq;
using System.Configuration;
using Messages;
using System.Text;

namespace RedisClient
{
    public class RedisCacheService : ICacheService
    {
        private Command _cmd;
        private static string _connection = ConfigurationManager.AppSettings["RedisConnection"];

        public RedisCacheService(Command cmd)
        {
            _cmd = cmd;
        }

        public Task<bool> Exists() =>
            Get(instance => instance.KeyExistsAsync(_cmd.Key));

        public Task<string> Get()
        {
            return Get(async instance =>
            {
                var theType = await instance.KeyTypeAsync(_cmd.Key);
                switch (theType)
                {
                    case RedisType.Set:
                        var str = new StringBuilder();
                        var scan = instance.SetScan(_cmd.Key);
                        foreach (var val in scan)
                        {
                            str.Append(val.ToString());
                            str.Append(",");
                        }
                        return str.ToString();
                }
                var redisValue = await instance.StringGetAsync(_cmd.Key);

                return redisValue.ToString();
            });
        }

        public Task Remove() =>
            Do(instance => instance.GetDatabase(_cmd.DatabaseId.Value).KeyDeleteAsync(_cmd.Key));
        
        public async Task Clear() =>
            await Do(async instance =>
            {
                var endpoints = instance.GetEndPoints(true);
                var tasks = endpoints.Select(x => instance.GetServer(x).FlushDatabaseAsync(_cmd.DatabaseId.Value));
                await Task.WhenAll(tasks);
            });

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
                return await fn(instance.GetDatabase(_cmd.DatabaseId.Value));
            }
        }
    }
}

using System.Threading.Tasks;

namespace RedisClient
{
    public interface ICacheService
    {
        Task<bool> Exists(string key);
        Task<string> Get(string key);
        Task Remove(string key);
        Task Clear();
    }
}
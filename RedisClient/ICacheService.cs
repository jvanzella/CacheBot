using System.Threading.Tasks;

namespace RedisClient
{
    public interface ICacheService
    {
        Task<bool> Exists();
        Task<string> Get();
        Task Remove();
        Task Clear();
    }
}
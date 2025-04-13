using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.RedisCache.Interface
{
    public interface IRedisCacheService
    {
        Task SetAsync<T>(string key, T value);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task<T?> GetObjectAsync<T>(string key);
        Task<bool> RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<bool> SetExpirationAsync(string key, TimeSpan expiration);
    }
}

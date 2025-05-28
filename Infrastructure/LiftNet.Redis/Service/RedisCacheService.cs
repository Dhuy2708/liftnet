using LiftNet.Redis.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Redis.Service
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task SetAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var jsonValue = JsonConvert.SerializeObject(value);
            var byteValue = Encoding.UTF8.GetBytes(jsonValue);
            await _cache.SetAsync(key, byteValue);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var jsonValue = JsonConvert.SerializeObject(value);
            var byteValue = Encoding.UTF8.GetBytes(jsonValue);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            
            await _cache.SetAsync(key, byteValue, options);
        }

        public async Task<T?> GetObjectAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var byteValue = await _cache.GetAsync(key);
            if (byteValue == null)
                return default;

            var jsonValue = Encoding.UTF8.GetString(byteValue);
            return JsonConvert.DeserializeObject<T>(jsonValue);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                await _cache.RemoveAsync(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                var value = await _cache.GetAsync(key);
                return value != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetExpirationAsync(string key, TimeSpan expiration)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                var value = await _cache.GetAsync(key);
                if (value == null)
                    return false;

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                };

                await _cache.SetAsync(key, value, options);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

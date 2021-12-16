using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lounge.API
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;

        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            var cachedResponse = await _cache.GetStringAsync(key);
            return cachedResponse == null ? null : JsonConvert.DeserializeObject<T>(cachedResponse);
        }

        public async Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            var response = JsonConvert.SerializeObject(value);
            await _cache.SetStringAsync(key, response, options);
        }

        public async Task ClearCache(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}

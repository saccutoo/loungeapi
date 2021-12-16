using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lounge.API
{
    public interface IRedisService
    {
        Task<T> GetFromCache<T>(string key) where T : class;

        Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options);

        Task ClearCache(string key);
    }
}

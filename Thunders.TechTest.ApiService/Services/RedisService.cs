using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Thunders.TechTest.ApiService.Interfaces;

namespace Thunders.TechTest.ApiService.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _redisCache;

        public RedisService(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<string?> GetCacheByKey(string key)
        {
            return await _redisCache.GetStringAsync(key);
        }

        public async Task GenerateCache(object inputObject, string cacheKey)
        {
            await _redisCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(inputObject),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        }
    }
}
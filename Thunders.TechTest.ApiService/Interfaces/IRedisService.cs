namespace Thunders.TechTest.ApiService.Interfaces
{
    public interface IRedisService
    {
        Task<string?> GetCacheByKey(string key);

        Task GenerateCache(object inputObject, string cacheKey);
    }
}

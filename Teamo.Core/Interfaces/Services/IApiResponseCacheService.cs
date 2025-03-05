namespace Teamo.Core.Interfaces.Services
{
    public interface IApiResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
        Task<string> GetCachedResponseAsync(string cacheKey);
        Task RemoveCacheByPattern(string pattern);
    }
}

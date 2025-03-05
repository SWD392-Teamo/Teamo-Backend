using StackExchange.Redis;
using System.Text.Json;
using Teamo.Core.Interfaces.Services;

namespace Teamo.Infrastructure.Services
{
    public class ApiResponseCacheService : IApiResponseCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public ApiResponseCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        /// <summary>
        /// Cache the api response in key-value pairs
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="response"></param>
        /// <param name="timeToLive"></param>
        /// <returns></returns>
        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var serializedResponse = JsonSerializer.Serialize(response, options);

            await _database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
        }

        /// <summary>
        /// Get the cached response with cache key which is the API URL
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _database.StringGetAsync(cacheKey);

            if (cachedResponse.IsNullOrEmpty) return null;

            return cachedResponse;
        }

        /// <summary>
        /// Remove cache with regex pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public async Task RemoveCacheByPattern(string pattern)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(database: 0, pattern: $"*{pattern}*").ToArray();

            if (keys.Length != 0)
            {
                await _database.KeyDeleteAsync(keys);
            }
        }
    }
}

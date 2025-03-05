using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Teamo.Core.Interfaces.Services;

namespace TeamoWeb.API.RequestHelpers
{
    [AttributeUsage(AttributeTargets.All)]
    public class CacheAttribute(int timeToLiveSeconds) : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// This filter attribute is executed once the 
        /// method having this attribute is executed.
        /// Add a new entry into the cache database
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get API response cache service from HttpContext
            var cacheService = context.HttpContext.RequestServices
                .GetRequiredService<IApiResponseCacheService>();

            // Generate cache key
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            // Get cached response instance using cache key
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            // If the response data change the content result to the cache instead
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;

                return;
            }

            // Move on to the next request handler
            var executedContext = await next();

            // If the cache for the request does not exist then
            // cache the response of the new request
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                if (okObjectResult.Value != null)
                {
                    // Add key-value pair of cache with time span
                    // so it can dispose in a specified time
                    await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value,
                        TimeSpan.FromSeconds(timeToLiveSeconds));
                }
            }
        }

        /// <summary>
        /// Generate the cache key from Request object
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            // Get the path from the request object
            keyBuilder.Append($"{request.Path}");

            // Append queries in the url params in to the keybuilder
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            // Return the cache key
            return keyBuilder.ToString();
        }
    }
}

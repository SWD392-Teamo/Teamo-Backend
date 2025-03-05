using Microsoft.AspNetCore.Mvc.Filters;
using Teamo.Core.Interfaces.Services;

namespace TeamoWeb.API.RequestHelpers;

[AttributeUsage(AttributeTargets.Method)]
public class InvalidateCache(string pattern) : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// Remove the cache from redis database mainly used for 
    /// unecessary caching for post, patch, delete request
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Move on with the request handler
        var resultContext = await next();

        // If no exceptions caught then remove the cache
        if (resultContext.Exception == null || resultContext.ExceptionHandled)
        {
            // Get cache service from HttpContext
            var cacheService = context.HttpContext.RequestServices
                .GetRequiredService<IApiResponseCacheService>();

            // Invalidate the cache by removing it from redis database
            await cacheService.RemoveCacheByPattern(pattern);
        }
    }
}
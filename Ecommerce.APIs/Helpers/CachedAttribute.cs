using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. »‰ÃÌ» «·”—›Ì” » «⁄ ‰« „‰ «·‹ Container
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            // 2. »‰ﬂÊ‰ „› «Õ ··ﬂ«‘ »‰«¡ ⁄·Ï «·‹ URL Ê«·‹ Query String
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            // 3. »‰‘Ê› Â· ⁄‰œ‰« œ« « „ Œ“‰… ··„› «Õ œÂø
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            // 4. ·Ê ·ﬁÌ‰« œ« «° »‰—Ã⁄Â« ›Ê—« Ê„‘ »‰ﬂ„·  ‰›Ì– «·ﬂ‰ —Ê·—
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

            // 5. ·Ê „·ﬁÌ‰«‘° »‰Œ·Ì «·ﬂ‰ —Ê·— Ìﬂ„· ‘€·Â ⁄«œÌ
            var executedContext = await next();

            // 6. »⁄œ „« «·ﬂ‰ —Ê·— ÌŒ·’° »‰«Œœ «·‰ ÌÃ… Ê‰Œ“‰Â« ›Ì «·ﬂ«‘ ··„—… «·Ã«Ì…
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");

            // »‰— » «·‹ Query String ⁄‘«‰ ·Ê «· — Ì» «Œ ·› »” «·ﬁÌ„ ÂÌ ÂÌ° Ì»ﬁÏ ‰›” «·ﬂ«‘
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}
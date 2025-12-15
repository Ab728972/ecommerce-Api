using System;
using System.Threading.Tasks;

namespace Ecommerce.Core.Interfaces
{
    public interface IResponseCacheService
    {
        // œ«·… ⁄‘«‰ ‰Õ›Ÿ «·«” Ã«»… ›Ì «·ﬂ«‘
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);

        // œ«·… ⁄‘«‰ ‰ÃÌ» «·«” Ã«»… „‰ «·ﬂ«‘
        Task<string> GetCachedResponseAsync(string cacheKey);
    }
}
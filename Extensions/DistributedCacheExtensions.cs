using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShowsService.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetShowAsync<T>(this IDistributedCache cache, string showId, T data, 
                                                                    TimeSpan? absoluteExpireTime = null,
                                                                    TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions(); //Sets up the configuration for values going in

            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60); //TIMER can be pulled from a central location
            options.SlidingExpiration = unusedExpireTime; 

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(showId, jsonData, options);
        }

        public static async Task<T> GetShowAsync<T>(this IDistributedCache cache, string showId)
        {
            var jsonData = await cache.GetStringAsync(showId);

            if (jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
        public static String GenerateKey(this string mediaType)
        {
            if (mediaType == "TV" || mediaType == "ONA" || mediaType == "OVA")
            { return "Anime"; }
            else if (mediaType == "movie")
            { return "Movie"; }

            return "";
        }
    }
}

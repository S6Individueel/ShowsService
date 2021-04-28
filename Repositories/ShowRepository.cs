using Microsoft.Extensions.Caching.Distributed;
using ShowsService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using ShowsService.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using AnimeService.DTOs;

namespace ShowsService.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly IDistributedCache cache;
        public ShowRepository(IDistributedCache distributedCache)
        {
            cache = distributedCache;
        }
        public async Task<string> GetShowAsync(string id)
        {
            var result = await cache.GetShowAsync<String>(id);

            if (result is null)
            {
                result = "Does not exist";
            }
            return result;
        }

        public async Task<string> SetShowAsync(string id)
        {
            var result = await cache.GetShowAsync<String>(id);

            if (result is null)
            {
                await cache.SetShowAsync(id, result);
            }
            else
            {
                return "This show already exists";
            }
            return "Show is saved";
        }


        public async Task<string> GetTrendingAnimesAsync()
        {
            var result = await cache.GetShowAsync<String>("Anime");

            if (result is null) {result = "Does not exist";}
            return result;
        }

        public async Task<string> GetTrendingMoviesAsync()
        {
            var result = await cache.GetShowAsync<String>("Movie");

            if (result is null) { result = "Does not exist"; }
            return result;
        }
    }
}

using Microsoft.Extensions.Caching.Distributed;
using ShowsService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using ShowsService.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ShowsService.DTOs;
using Newtonsoft.Json;

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

        public async Task<string> SetShowAsync(string id, string shows)
        {
            var result = await cache.GetShowAsync<String>(id);

            if (result is null)
            {
                await cache.SetShowAsync(id, shows);
            }
            else
            {
                return "This show already exists";
            }
            return "Show is saved";
        }

        public async Task<IList<ShowDTO>> GetTrendingShowsAsync(string key)
        {
            var result = await cache.GetShowAsync<String>(key);

            if (result is null) { result = "Does not exist"; }

            IList<ShowDTO> showList = JsonConvert.DeserializeObject<List<ShowDTO>>(result);

            return showList;
        }

        public async Task<IList<ShowDTO>> HandleRedisFailure()
        {
            List<ShowDTO> shows = new List<ShowDTO>();

            return shows;
        }
    }
}

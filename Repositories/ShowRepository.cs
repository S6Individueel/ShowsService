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
        public async Task<string> GetShowsTrendingAsync(string key)
        {
            // TODO: Opslaan van een lijst (Anime) in redis en het ophalen daarvan.
            // Kijkende of ik een collectie op kan slaan van Trending --> [Anime], [Movies] --> [{ShowData per Anime/Movie}]
            // Hiermee kan ik dan een GetAllTrending met {key} Anime of {key} Movie doen om zo data te verkrijgen.  
            return "e";
        }

        public async Task<string> SetShowsTrendingAsync(string data)
        {
            IList<JToken> results = JObject.Parse(data)["shows.trending"].Children().ToList(); //Parses content, gets the "top" list and converts to list.

            if (results is null)
            {
                return "Error, incorrect show data";
            }
            foreach (JToken show in results)
            {
                ShowDTO topShow = show.ToObject<ShowDTO>();
                await cache.SetShowAsync(topShow.Id.ToString(), topShow);
            }

            return "Show data set in database!";
        }
    }
}

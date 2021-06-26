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
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;

namespace ShowsService.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly IDistributedCache cache;
        private readonly HttpClient _httpClient;
        private IMemoryCache _memoryCache;
        public ShowRepository(IDistributedCache distributedCache, IMemoryCache memoryCache, HttpClient httpClient)
        {
            _memoryCache = memoryCache;
            cache = distributedCache;
            _httpClient = httpClient;
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
            string result = "";
            IList<ShowDTO> showList;

            try
            {
                result = await cache.GetShowAsync<String>(key);
            }
            catch (Exception)
            {
                showList = (IList<ShowDTO>)await GetFromMemoryCacheAsync(key);
                return showList;
            } 

            showList = JsonConvert.DeserializeObject<List<ShowDTO>>(result);

            return showList;
        }

        private async Task<IEnumerable<ShowDTO>> GetFromMemoryCacheAsync(string key)
        {
            IEnumerable<ShowDTO> cacheEntry;

            if (!_memoryCache.TryGetValue(key, out cacheEntry)) //If not in inMemory cache already
            {
                // Keep in cache for this time, reset time if accessed.
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));

                if (key.Equals("Anime"))
                {
                    IEnumerable<ShowDTO> animes = (await CallAnime())
                                    .Select(anime => anime.AsShowDTO());
                    _memoryCache.Set(key, animes, cacheEntryOptions);
                }
                else
                {
                    IEnumerable<ShowDTO> movies = (await CallMovie())
                                    .Select(movie => movie.AsShowDTO());
                    _memoryCache.Set(key, movies, cacheEntryOptions);
                }
            }
            return _memoryCache.Get<IEnumerable<ShowDTO>>(key).ToList();
        }

        private async Task<IEnumerable<TopAnime>> CallAnime()
        {
            string uri = "https://api.jikan.moe/v3/top/anime/0/airing";

            HttpResponseMessage response = await _httpClient.GetAsync(uri);
            var content = await response.Content.ReadAsStringAsync();

            IList<JToken> results = JObject.Parse(content)["top"].Children().ToList(); //Parses content, gets the "top" list and converts to list.

            IList<TopAnime> topAnimes = new List<TopAnime>();

            for (int animeCount = 0; animeCount < 10; animeCount++)
            {
                TopAnime topAnime = results[animeCount].ToObject<TopAnime>();
                topAnimes.Add(topAnime);
            }
            return topAnimes;
        }

        private async Task<IEnumerable<TopMovie>> CallMovie()
        {
            string uri = "https://api.themoviedb.org/3/trending/movie/week?api_key=3caffe903f7c34234eb189d6db9544fc";

            HttpResponseMessage response = await _httpClient.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();

                IList<JToken> results = JObject.Parse(content)["results"].Children().ToList();

                IList<TopMovie> topMovies = new List<TopMovie>();

                for (int movieCount = 0; movieCount < 10; movieCount++)
                {
                    TopMovie topAnime = results[movieCount].ToObject<TopMovie>();
                    topMovies.Add(topAnime);
                }
            return topMovies;
        }
    }
}

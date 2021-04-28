using AnimeService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShowsService.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsService.Controllers
{
    [Route("home")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IDistributedCache cache;
        public HomeController(IDistributedCache distributedCache)
        {
            cache = distributedCache;
        }

        [HttpGet("testcache/{id}")]
        public async Task<String> TestCacheAsync(string id)
        {
            var result = await cache.GetShowAsync<String>(id);
            
            if (result is null)
            {
                result = "API";
                await cache.SetShowAsync(id, id);
            } else
            {
                var get = await cache.GetShowAsync<String>(id);
                return get.ToString();
            }
            return result;
        }

        [HttpGet("get/{showType}")]
        public async Task<String> GetTestAsync(string showType)
        {
            var result = await cache.GetShowAsync<String>(showType);

            if (result is null)
            {
                return "null";
            }else
            {
                Console.WriteLine(result);
                return result;
            }
            return "emtpy";
            /*            foreach (JToken show in results)
            {
                TopAnime topAnime = anime.ToObject<TopAnime>();
                topAnimes.Add(topAnime);
            }
            await cache.SetShowAsync<String>(result);*/
        }

        [HttpPut("settest")]
        public async Task<string> PutTestAsync()
        {
            string body;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }

            List<ShowDTO> trendingShows = JsonConvert.DeserializeObject<List<ShowDTO>>(body);
            await cache.SetShowAsync<String>(KeyGenerator(trendingShows[0].Media_type), body, TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(60));
            
            return "";
        }

        public String KeyGenerator(string mediaType)
        {
            if (mediaType == "TV" || mediaType == "ONA" || mediaType == "OVA")
            { return "Anime"; }
            else if (mediaType == "movie")
            { return "Movie"; }

            return "";
        } 
    }
}

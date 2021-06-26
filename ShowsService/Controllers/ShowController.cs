using ShowsService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShowsService.Extensions;
using ShowsService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShowsService.Controllers
{
    [Route("show")]
    [ApiController]
    public class ShowController : Controller
    {
        private readonly IShowRepository _showRepository;
        public ShowController(IShowRepository showRepository)
        {
            _showRepository = showRepository;
        }

        [HttpGet("trending/anime")]
        public async Task<IList<ShowDTO>> GetTrendingAnimesAsync()
        {
            return await _showRepository.GetTrendingShowsAsync("Anime");
        }
        
        [HttpGet("trending/movie")]
        public async Task<IList<ShowDTO>> GetTrendingMoviesAsync()
        {
            return await _showRepository.GetTrendingShowsAsync("Movie");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowsService.Repositories.Interfaces
{
    public interface IShowRepository
    {
        Task<string> SetShowAsync(string id);
        Task<string> GetShowAsync(string id);
        Task<string> GetTrendingAnimesAsync();
        Task<string> GetTrendingMoviesAsync();
    }
}

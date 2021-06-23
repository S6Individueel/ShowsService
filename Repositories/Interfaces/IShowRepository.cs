using ShowsService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowsService.Repositories.Interfaces
{
    public interface IShowRepository
    {
        Task<string> SetShowAsync(string id, string shows);
        Task<string> GetShowAsync(string id);
        Task<IList<ShowDTO>> GetTrendingShowsAsync(string key);
    }
}

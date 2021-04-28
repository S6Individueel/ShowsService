using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowsService.Repositories.Interfaces
{
    interface IShowRepository
    {
        Task<string> SetShowAsync(string id);
        Task<string> GetShowAsync(string id);
        Task<string> SetShowsTrendingAsync(string data);
        Task<string> GetShowsTrendingAsync(string key);
    }
}

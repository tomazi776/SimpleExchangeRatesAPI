using DataLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public interface ICachingHelper
    {
        public HashSet<string> KeysToLookup { get; set; }
        Task SaveDataToCache(List<ICurrencyModel> data);
        Task<List<string>> LoadDataFromCache(ICurrencyModel data);
    }
}

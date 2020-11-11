using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public interface ICachingHelper
    {
        //Task<List<ICurrencyModel>> MapRequestData(ICurrencyModel model);
        Task SaveDataToCache(ICurrencyModel data);
        Task<IList<ICurrencyModel>> LoadDataFromCache(ICurrencyModel data);
    }
}

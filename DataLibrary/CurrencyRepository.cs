using DataLibrary.Models;
using DataLibrary.Services;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class CurrencyRepository : ICurrencyRepository
    {
        //private IConnectionProvider _connProvider;
        private IDistributedCache _distributedCache;
        private ICachingHelper _cachingHelper;

        public CurrencyRepository(IDistributedCache distributedCache, ICachingHelper cachingHelper)
        {
            _distributedCache = distributedCache;
            _cachingHelper = cachingHelper;

        }

        public async Task<IList<ICurrencyModel>> GetData( ICurrencyModel model)
        {
            IList<ICurrencyModel> cachedData;

            //Gets from server and caches
            await _cachingHelper.SaveDataToCache(model);
            cachedData = await _cachingHelper.LoadDataFromCache(model);
            return cachedData;
        }

        public int SaveData<T>()
        {
            throw new NotImplementedException();
        }
    }
}

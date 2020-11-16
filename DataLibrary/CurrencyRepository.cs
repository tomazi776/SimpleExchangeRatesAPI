using DataLibrary.Models;
using DataLibrary.Services;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class CurrencyRepository : ICurrencyRepository
    {
        //private IConnectionProvider _connProvider;
        private readonly IDistributedCache _distributedCache;
        private readonly ICachingHelper _cachingHelper;
        private readonly IResponseDataProcessor _responseDataProcessor;
        private readonly IRequestManager _requestManager;

        public CurrencyRepository(IDistributedCache distributedCache, ICachingHelper cachingHelper,
            IResponseDataProcessor responseDataProcessor, IRequestManager requestManager)
        {
            _distributedCache = distributedCache;
            _cachingHelper = cachingHelper;
            _responseDataProcessor = responseDataProcessor;
            _requestManager = requestManager;
        }

        public async Task<IList<string>> GetData(ICurrencyModel model, string requestUrl)
        {
            //IList<ICurrencyModel> cachedData;
            IList<string> cachedData;

            _cachingHelper.KeysToLookup.Add("PLN2020_11_05");
            _cachingHelper.KeysToLookup.Add("PLN2020_11_06");

            cachedData = await _cachingHelper.LoadDataFromCache(model);

            if (cachedData.Any(item => item is null))
            {
                //TODO:
                //Get only data for keys wich got null
                //var dataToFetch = cachedData.GetDifferenceBetweenCachedAndToLookup();

                //Gets data for specified keys from ECB
                //string jSon = await _cachingHelper.SendRequest(dataToFetch);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                //TODO: move responsibility from _cachingHelper to RequestHandler
                string jSon = await _requestManager.SendRequest();
                var processedData = _responseDataProcessor.Deserialize(model, jSon);
                sw.Stop();
                Console.WriteLine("TIME IT TOOK TO FETCH DATA FROM ECB (Miliseconds): " + sw.ElapsedMilliseconds); ;

                await _cachingHelper.SaveDataToCache(processedData);
                cachedData = await _cachingHelper.LoadDataFromCache(model);
            }

            return cachedData;
        }

        public int SaveData<T>()
        {
            throw new NotImplementedException();
        }
    }
}

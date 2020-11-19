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
        private readonly ICachingHelper _cachingHelper;
        private readonly IResponseDataProcessor _responseDataProcessor;
        private readonly IRequestManager _requestManager;

        public CurrencyRepository(ICachingHelper cachingHelper,
            IResponseDataProcessor responseDataProcessor, IRequestManager requestManager)
        {
            _cachingHelper = cachingHelper;
            _responseDataProcessor = responseDataProcessor;
            _requestManager = requestManager;
        }

        public async Task<IList<string>> GetData(ICurrencyModel model, string endpoint)
        {
            IList<string> cachedData;
            AddLookupKeysFromEndpoint(endpoint);

            cachedData = await _cachingHelper.LoadDataFromCache(model);
            if (cachedData.Any(item => item is null))
            {
                //TODO:
                //Get only data for keys wich got null
                //var dataToFetch = cachedData.GetDifferenceBetweenCachedAndToLookup();

                Stopwatch sw = new Stopwatch();
                sw.Start();
                string jSon = await _requestManager.SendGetRequest(endpoint);
                var processedData = _responseDataProcessor.Deserialize(model, jSon);
                sw.Stop();
                Console.WriteLine("TIME IT TOOK TO FETCH DATA FROM ECB (Miliseconds): " + sw.ElapsedMilliseconds); ;

                await _cachingHelper.SaveDataToCache(processedData);
                cachedData = await _cachingHelper.LoadDataFromCache(model);
            }
            return cachedData;
        }

        private void AddLookupKeysFromEndpoint(string endpoint)
        {
            var lookupKeys = _requestManager.MapEndpointToLookupKeys(endpoint);
            foreach (var key in lookupKeys)
            {
                _cachingHelper.KeysToLookup.Add(key);
            }
        }
    }
}

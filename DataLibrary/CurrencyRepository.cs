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
            _cachingHelper.NotFoundKeys.Clear();

            AddLookupKeysFromEndpoint(endpoint);

            //Set single last request endpoint to this request's endpoint
            _requestManager.SetLastRequestEndpoint(endpoint);

            cachedData = await _cachingHelper.LoadDataFromCache(model);

            //some data is missing, so get what's missing
            if (cachedData.Any(item => item is null) || cachedData.Count.Equals(0))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var missingData = GetDataForMissingTimeFrame(model, _cachingHelper.NotFoundKeys);
                sw.Stop();
                Console.WriteLine("TIME IT TOOK TO FETCH AND DESERIALIZE DATA FROM ECB (Miliseconds): " + sw.ElapsedMilliseconds);

                await _cachingHelper.SaveDataToCache(missingData);
                cachedData = await _cachingHelper.LoadDataFromCache(model);
            }
            return cachedData;
        }

        private List<ICurrencyModel> GetDataForMissingTimeFrame(ICurrencyModel model, HashSet<string> notFoundKeys)
        {
            var missingTimeFrameEndpoint = _requestManager.CreateEndpointForMissingTimeFrame(notFoundKeys);
            string jSon = _requestManager.SendGetRequest(missingTimeFrameEndpoint).Result;
            var processedData = _responseDataProcessor.Deserialize(model, jSon);
            return processedData;
        }

        private void AddLookupKeysFromEndpoint(string endpoint)
        {
            var lookupKeys = _requestManager.MapEndpointToLookupKeys(endpoint);

            _cachingHelper.LookupKeys.Clear();
            foreach (var key in lookupKeys)
            {
                _cachingHelper.LookupKeys.Add(key);
            }
        }
    }
}

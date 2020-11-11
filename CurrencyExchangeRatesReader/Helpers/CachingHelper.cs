using CurrencyExchangeRatesReader.Services;
using DataLibrary.Extensions;
using DataLibrary.Models;
using DataLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Helpers
{
    public class CachingHelper : ICachingHelper
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IResponseDataProcessor _responseDataProcessor;


        //public bool IsInCache { get; set; }

        public List<string> KeysToLookup { get; private set; } = new List<string>();

        public CachingHelper(IDistributedCache distributedCache, IResponseDataProcessor responseDataProcessor)
        {
            _distributedCache = distributedCache;
            _responseDataProcessor = responseDataProcessor;
        }

        //public bool CheckIfInCache()
        //{
        //    return IsInCache = LookupKeys.Any();
        //}

        public async Task SaveDataToCache(ICurrencyModel model)
        {
            //Gets data from ECB
            string jSon = await SendRequest();
            var processedData = _responseDataProcessor.Deserialize(model, jSon);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var item in processedData)
            {
                var recordId = item.CreateId();
                KeysToLookup.Add(recordId);

                //TODO: make optional expire times depend upon data calling frequency
                await _distributedCache.SetRecordAsync(item, recordId);
            }
            stopwatch.Stop();
            System.Console.WriteLine("SAVING DATA --- TIME IN MILISECONDS: " + stopwatch.ElapsedMilliseconds);

            //if (KeysToLookup.Count > 0)
            //{
            //    foreach (var item in processedData)
            //    {
            //        var id = item.CreateId();

            //        //TODO: make optional expire times depend upon data calling frequency
            //        await _distributedCache.SetRecordAsync(item, id);
            //    }
            //}
        }

        public async Task<IList<ICurrencyModel>> LoadDataFromCache(ICurrencyModel data)
        {
            IList<ICurrencyModel> records = new List<ICurrencyModel>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            foreach (var recordId in KeysToLookup)
            {
                // TODO: notify (maybe other thread) that single record not found to generate Request and go get it. 
                // When finished loading, check again.
                var record = await _distributedCache.GetRecordAsync<ICurrencyModel>(recordId);
                records.Add(record);
            }
            stopWatch.Stop();
            System.Console.WriteLine("LOADING DATA --- TIME IN MILISECONDS: " + stopWatch.ElapsedMilliseconds);
            return records;
        }

        private async Task<string> SendRequest()
        {
            HttpResponseMessage response;
            HttpRequestMessage request = CreateRequest("https://sdw-wsrest.ecb.europa.eu/service/data/EXR/D.PLN+USD.EUR.SP00.A?startPeriod=2020-11-05&detail=dataonly");
            using (var client = new HttpClient())
            {
                response = await client.SendAsync(request);
            }

            //Handle if not
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        private HttpRequestMessage CreateRequest(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("Accept", "application/json");
            req.Headers.Add("Accept-Encoding", "deflate");
            return req;
        }
    }
}

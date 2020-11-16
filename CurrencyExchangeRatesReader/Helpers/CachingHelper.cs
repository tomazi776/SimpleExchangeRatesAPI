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
        public List<string> KeysToLookup { get; set; } = new List<string>();

        public CachingHelper(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SaveDataToCache(List<ICurrencyModel> data)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            KeysToLookup.Clear();
            foreach (var item in data)
            {
                var recordId = item.CreateId();
                KeysToLookup.Add(recordId);

                //TODO: make optional expire times depend upon data calling frequency
                await _distributedCache.SetRecordAsync(item, recordId);
            }
            stopwatch.Stop();
            System.Console.WriteLine("SAVING DATA --- TIME IN MILISECONDS: " + stopwatch.ElapsedMilliseconds);
        }

        public async Task<List<string>> LoadDataFromCache(ICurrencyModel data)
        {
            List<string> records = new List<string>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            foreach (var recordId in KeysToLookup)
            {
                var record = await _distributedCache.GetRecordAsync<Currency>(recordId);
                records.Add(record);
            }
            stopWatch.Stop();
            System.Console.WriteLine("LOADING DATA --- TIME IN MILISECONDS: " + stopWatch.ElapsedMilliseconds);
            return records;
        }
    }
}

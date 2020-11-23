using DataLibrary;
using DataLibrary.Extensions;
using DataLibrary.Models;
using DataLibrary.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Helpers
{
    public class CachingHelper : ICachingHelper
    {
        private readonly IDistributedCache _distributedCache;
        public HashSet<string> KeysToLookup { get; set; } = new HashSet<string>();
        public HashSet<string> NotFoundKeys { get; set; } = new HashSet<string>();

        public CachingHelper(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SaveDataToCache(List<ICurrencyModel> data)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var item in data)
            {
                var recordId = item.CreateId();

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
                if (record is null && !IsHoliday(recordId))
                {
                    NotFoundKeys.Add(recordId);
                }
                if (!IsHoliday(recordId))
                {
                    records.Add(record);
                }
            }
            stopWatch.Stop();
            System.Console.WriteLine("LOADING DATA --- TIME IN MILISECONDS: " + stopWatch.ElapsedMilliseconds);
            return records;
        }

        private bool IsHoliday(string recordId)
        {
            return SingleLastRequest.Instance.Holidays.Contains(recordId);
        }
    }
}

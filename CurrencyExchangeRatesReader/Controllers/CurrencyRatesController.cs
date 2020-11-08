using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DataLibrary.Models;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchangeRatesReader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CurrencyRatesController : ControllerBase
    {
        private IEasyCachingProviderFactory _cachingProviderFactory;
        private IEasyCachingProvider _cachingProvider;
        const string serviceBaseAddress = @"https://sdw-wsrest.ecb.europa.eu/service/data/EXR/";
        const string ratesForPlnEurSinceFourthNovember = "D.PLN.EUR.SP00.A?startPeriod=2020-11-04";

        public CurrencyRatesController(IEasyCachingProviderFactory cachingProviderFactory)
        {
            //Add Run Redis server if not running already
            //ServiceController.GetServices - smth like that

            _cachingProviderFactory = cachingProviderFactory;
            _cachingProvider = _cachingProviderFactory.GetCachingProvider("redis1");
        }

        [HttpGet("Get/PLN_EUR_FromFourthNov")]
        public void GetExchangeRatesForPLN()
        {
            var provider = CultureInfo.InvariantCulture;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(serviceBaseAddress + ratesForPlnEurSinceFourthNovember);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip, deflate"));

                var jsonData = client.GetStringAsync(ratesForPlnEurSinceFourthNovember).Result;
            }

            var data = new CurrencyRateModel()
            {
                Code = "PLN",
                ExchangeRate = 4.532d,
                DateRange = new List<DateTime>()
            };

            data.DateRange.Add(DateTime.ParseExact("2020-11-04", "yyyy-MM-dd", provider));

            //Setting expiration date should be dependant on date - longer for monthly data, short for days
            _cachingProvider.Set("D_PLN_EUR_Start_2020-11-04", data, TimeSpan.FromMinutes(60));
        }

        // GET: api/<CurrencyRatesController>
        [HttpGet("Get")]
        public IEnumerable<string> GetAllRates()
        {
            return new string[] { "value1", "value2" };
        }
    }
}

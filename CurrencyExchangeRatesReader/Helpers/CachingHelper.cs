using CurrencyExchangeRatesReader.JsonModels;
using CurrencyExchangeRatesReader.Services;
using DataLibrary.Models;
using EasyCaching.Core;
using EasyCaching.Core.Interceptor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Helpers
{
    public class CachingHelper : ICachingHelper
    {
        private readonly IEasyCachingProviderFactory _cachingProviderFactory;
        private readonly IEasyCachingProvider _easyCachingProvider;

        public CachingHelper(IEasyCachingProviderFactory easyCachingProviderFactory)
        {
            _cachingProviderFactory = easyCachingProviderFactory;
            _easyCachingProvider = _cachingProviderFactory.GetCachingProvider("redis1");
        }

        public async Task<List<ICurrencyModel>> MapRequestData(ICurrencyModel dataModel)
        {
            List<ICurrencyModel> currencyrates = new List<ICurrencyModel>();

            string jSon = string.Empty;
            jSon = await SendRequest();

            JObject parsedJson = JObject.Parse(jSon);

            //Refactor to support wildcarding for multiple currencies
            var currenciesNodes = parsedJson["dataSets"].First["series"].Children();

            // different for dataonly
            var currenciesInfoNodes = parsedJson["structure"]["dimensions"]["series"][1].Values().Children();
            var dateRanges = parsedJson["structure"]["dimensions"]["observation"][0].Values().Children();

            List<string> codes = new List<string>();
            List<string> names = new List<string>();
            List<DateTime> dates = new List<DateTime>();
            AddCurrenciesInfo(codes, names, currenciesInfoNodes, dates, dateRanges);

            int i = 0;
            foreach (var curr in currenciesNodes)
            {
                int j = 0;
                var timeFrameCurrencyNodes = currenciesNodes.Children().ToList()[i].Children().Last().Children().ToList().Children();
                foreach (var currencyNode in timeFrameCurrencyNodes)
                {
                    var eRPerTimeFrame = currencyNode.Children().First().Children().ToList()[0].Value<decimal>();
                    dataModel = new DataLibrary.Models.Currency()
                    {
                        Code = codes[i],
                        Name = names[i],
                        ExchangeRate = eRPerTimeFrame,
                        ObservationDate = dates[j]
                    };

                    // For testing
                    Console.WriteLine(dataModel.Code + "(" + dataModel.Name + ")" +Environment.NewLine +
                        dataModel.ExchangeRate.ToString() + Environment.NewLine + 
                        dataModel.ObservationDate.ToString() + Environment.NewLine);

                    currencyrates.Add(dataModel);
                    j++;
                }
                i++;
            }
            return currencyrates;
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

        private void AddCurrenciesInfo(List<string> codes, List<string> names, IJEnumerable<JToken> infoNodes, List<DateTime> dates, IJEnumerable<JToken> dateRanges)
        {
            foreach (var infoNode in infoNodes)
            {
                codes.Add(infoNode["id"].Value<string>());
                names.Add(infoNode["name"].Value<string>());
            }
            foreach (var date in dateRanges)
            {
                dates.Add(date["id"].Value<DateTime>());
            }
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

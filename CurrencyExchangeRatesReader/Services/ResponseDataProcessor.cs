using DataLibrary.Models;
using DataLibrary.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyExchangeRatesReader.Services
{
    public class ResponseDataProcessor : IResponseDataProcessor
    {
        public List<ICurrencyModel> Deserialize(ICurrencyModel dataModel, string json)
        {
                List<ICurrencyModel> currencyrates = new List<ICurrencyModel>();

                //Process data
                JObject parsedJson = JObject.Parse(json);

                //Refactor to support wildcarding for multiple currencies
                var currenciesNodes = parsedJson["dataSets"].First["series"].Children();

                // different for detail=full
                var currenciesInfoNodes = parsedJson["structure"]["dimensions"]["series"][1].Values().Children();
                var dateRanges = parsedJson["structure"]["dimensions"]["observation"][0].Values().Children();

                List<string> codes = new List<string>();
                List<string> names = new List<string>();
                List<DateTime> dates = new List<DateTime>();
                AddCurrenciesInfo(codes, names, currenciesInfoNodes, dates, dateRanges);

                for (int i = 0; i < currenciesNodes.Count(); i++)
                {
                    int j = 0;
                    var timeFrameCurrencyNodes = currenciesNodes.Children().ToList()[i].Children().Last().Children().ToList().Children();
                    foreach (var currencyNode in timeFrameCurrencyNodes)
                    {
                        var eRPerTimeFrame = currencyNode.Children().First().Children().ToList()[0].Value<decimal>();
                         dataModel = new Currency()
                        {
                            Code = codes[i],
                            Name = names[i],
                            ExchangeRate = eRPerTimeFrame,
                            ObservationDate = dates[j]
                        };

                        // For testing
                        Console.WriteLine(dataModel.Code + "(" + dataModel.Name + ")" + Environment.NewLine +
                            dataModel.ExchangeRate.ToString() + Environment.NewLine +
                            dataModel.ObservationDate.ToString() + Environment.NewLine);

                        currencyrates.Add(dataModel);
                        j++;
                    }
                }
                return currencyrates;
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
    }
}

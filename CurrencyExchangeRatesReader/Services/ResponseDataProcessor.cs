using CurrencyExchangeRatesReader.Helpers;
using DataLibrary;
using DataLibrary.Extensions;
using DataLibrary.Models;
using DataLibrary.Services;
using Newtonsoft.Json;
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
            JObject parsedJson = JObject.Parse(json);

            //Refactor to support wildcarding for multiple currencies
            var currenciesNodes = parsedJson["dataSets"].First["series"].Children();

            // different for detail=full
            var currenciesInfoNodes = parsedJson["structure"]["dimensions"]["series"][1].Values().Children();
            var dateRanges = parsedJson["structure"]["dimensions"]["observation"][0].Values().Children();

            List<string> codes = new List<string>();
            List<string> names = new List<string>();
            List<DateTime> dates = new List<DateTime>();

            AddCurrenciesCodesNames(codes, names, currenciesInfoNodes);
            AddCurrenciesDates(dates, dateRanges);

            var currencies = new List<ICurrencyModel>();
            for (int i = 0; i < currenciesNodes.Count(); i++)
            {
                var timeFrameCurrencyNodes = currenciesNodes.Children().ToList()[i].Children().Last().Children().ToList().Children();
                currencies = GetMappedDataForTimeFrames(codes, names, dates, i, timeFrameCurrencyNodes);
            }
            return currencies;
        }

        private List<ICurrencyModel> GetMappedDataForTimeFrames(List<string> codes, List<string> names, List<DateTime> dates, int i, IJEnumerable<JToken> timeFrameCurrencyNodes)
        {
            List<ICurrencyModel> currencyrates = new List<ICurrencyModel>();
            for (int j = 0; j < timeFrameCurrencyNodes.Count(); j++)
            {
                var currencyNode = timeFrameCurrencyNodes.ElementAt(j);
                if (IsHoliday(currencyNode))
                {
                    var holidayId = CreateHolidayId(codes, names, dates, i, j);
                    SingleLastRequest.Instance.Holidays.Add(holidayId);
                    continue;
                }
                // TODO: Refactor
                var exchangeRate = currencyNode.Children().First().Children().ToList()[0].Value<decimal>();
                var dataModel = MapToDataModel(codes, names, dates, i, j, exchangeRate);

                // Print for testing 
                // TODO: Handle later with Logger
                PrintDeserializedData(dataModel);
                currencyrates.Add(dataModel);
            }
            return currencyrates;
        }

        private string CreateHolidayId(List<string> codes, List<string> names, List<DateTime> dates, int i, int j)
        {
            var holidayId = codes[i] + "_" + dates[j].ToString("yyyy_MM_dd");
            return holidayId;
        }

        private static void PrintDeserializedData(ICurrencyModel dataModel)
        {
            Console.WriteLine("DESERIALIZED DATA:" + dataModel.Code + "(" + dataModel.Name + ")" + Environment.NewLine +
                dataModel.ExchangeRate.ToString() + Environment.NewLine +
                dataModel.ObservationDate.ToString() + Environment.NewLine);
        }

        private static ICurrencyModel MapToDataModel(List<string> codes, List<string> names, List<DateTime> dates, int i, int j, decimal eRPerTimeFrame)
        {

            var currencyItem = new  Currency()
            {
                Code = codes[i],
                Name = names[i],
                ExchangeRate = eRPerTimeFrame,
                ObservationDate = dates[j].ToString("yyyy_MM_dd")
            };
            var recordId = currencyItem.CreateId();
            SingleLastRequest.Instance.LookupKeys.Add(recordId);
            return currencyItem;
        }

        private void AddCurrenciesCodesNames(List<string> codes, List<string> names, IJEnumerable<JToken> infoNodes )
        {
            foreach (var infoNode in infoNodes)
            {
                codes.Add(infoNode["id"].Value<string>());
                names.Add(infoNode["name"].Value<string>());
            }
        }

        private void AddCurrenciesDates(List<DateTime> dates, IJEnumerable<JToken> dateRanges)
        {
            foreach (var date in dateRanges)
                dates.Add(date["id"].Value<DateTime>());
        }

        private bool IsHoliday(JToken currencyNode)
        {
            return currencyNode.Children().First().ElementAt(1).Value<int>() == 1;
        }
    }
}

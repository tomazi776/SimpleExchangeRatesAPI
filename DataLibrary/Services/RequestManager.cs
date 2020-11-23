using DataLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public class RequestManager : IRequestManager
    {
        private const string BaseAddress = "https://sdw-wsrest.ecb.europa.eu/service/data/EXR/";
        public async Task<string> SendGetRequest(string endpoint)
        {
            HttpRequestMessage getRequest = CreateGetRequest(endpoint);
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                response = await client.SendAsync(getRequest);
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }


        public List<string> MapEndpointToLookupKeys(string endpoint)
        {
            List<string> lookupKeys;

            var queryData = endpoint.Split('.', '+', '?', '=', '&');
            List<DateTime> startEndDates = new List<DateTime>();
            List<string> codes;

            MapTimePeriods(queryData, startEndDates, "startPeriod");
            MapTimePeriods(queryData, startEndDates, "endPeriod");

            var dates = CreateRange(startEndDates);

            codes = MapCurrencyCodes(queryData);
            lookupKeys = CombineDatesWithCodes(dates, codes);
            return lookupKeys;
        }

        private List<string> CreateRange(List<DateTime> dates)
        {
            var datesRange = CreateRangeForWorkingDays(dates[0], dates[1]);
            return FormatDatesToStrings(datesRange);
        }

        public string CreateEndpointForMissingTimeFrame(HashSet<string> keys)
        {
            string missingDataEndpoint = string.Empty;
            var codes = GetCodesData(keys);

            var delimitedCodes = ApplyDelimiters(codes);
            var startDate = GetDatePart(keys.First());
            var endDate = GetDatePart(keys.Last());

            StringBuilder sb = new StringBuilder();
            missingDataEndpoint = sb
                     .Append(EndpointHelper.dailyFrequency)
                     .Append(delimitedCodes)
                     .Append(EndpointHelper.denominatedInEur)
                     .Append(EndpointHelper.referenceRatesCode)
                     .Append(EndpointHelper.variationMeasure)
                     .Append(EndpointHelper.optionalParamChar)
                     .Append("startPeriod=")
                     .Append(startDate)
                     .Append("&")
                     .Append("endPeriod=")
                     .Append(endDate).ToString();
            return missingDataEndpoint;
        }

        private string ApplyDelimiters(HashSet<string> codes)
        {
            List<string> newCodes = new List<string>(codes);
            var onlyCodes = string.Empty;
            var output = string.Empty;
            if (newCodes.Count > 1)
            {
                foreach (var code in codes)
                {
                    onlyCodes += code + "+";
                }
                //int trailingIndex = onlyCodes.LastIndexOf('+')-1;
                output = onlyCodes.TrimEnd('+');
            }
            else
            {
                output = newCodes[0];
            }
            return output;
        }

        private HashSet<string> GetCodesData(HashSet<string> keys)
        {
            HashSet<string> codes = new HashSet<string>();
            foreach (var key in keys)
            {
                var keyData = key.Split('_');
                var codePart = keyData[0];
                codes.Add(codePart);
            }
            return codes;
        }

        private string GetDatePart(string key)
        {
            var keyData = key.Split('_');
            string date = keyData[1] + "-" + keyData[2] + "-"+ keyData[3];
            return date;
        }

        private static List<string> FormatDatesToStrings(List<DateTime> allDates)
        {
            List<string> allDatesInStrings = new List<string>();
            foreach (var date in allDates)
            {
                var dateInString = date.ToString("yyyy_MM_dd");
                allDatesInStrings.Add(dateInString);
            }
            return allDatesInStrings;
        }

        private static List<DateTime> CreateRangeForWorkingDays(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    allDates.Add(date);
                }
            }
            return allDates;
        }

        private List<string> CombineDatesWithCodes(List<string> dates, List<string> codes)
        {
            List<string> lookupKeys = new List<string>();

            foreach (var date in dates)
            {
                foreach (var code in codes)
                {
                    var key = code + "_" + date;
                    lookupKeys.Add(key);
                }
            }
            return lookupKeys;
        }

        private List<string> MapCurrencyCodes(string[] queryData)
        {
            List<string> codes = new List<string>();
            var index = Array.FindIndex(queryData, item => item == "SP00");
            index--;

            //skip first
            while (index > 1)
            {
                index--;
                codes.Add(queryData[index]);
            }

            // support wildcard to get all currencies
            // TODO: refactor to get only codes data from ECB
            if (codes.First() == string.Empty)
            {
                codes.RemoveAt(0);
                codes.Add("AUD");
                codes.Add("BGN");
                codes.Add("BRL");
                codes.Add("CAD");
                codes.Add("CHF");
                codes.Add("CNY");
                codes.Add("CZK");
                codes.Add("DKK");
                codes.Add("GBP");
                codes.Add("HKD");
                codes.Add("HRK");
                codes.Add("HUF");
                codes.Add("IDR");
                codes.Add("ILS");
                codes.Add("INR");
                codes.Add("ISK");
                codes.Add("JPY");
                codes.Add("KRW");
                codes.Add("MXN");
                codes.Add("MYR");
                codes.Add("NOK");
                codes.Add("NZD");
                codes.Add("PHP");
                codes.Add("PLN");
                codes.Add("RON");
                codes.Add("RUB");
                codes.Add("SEK");
                codes.Add("SGD");
                codes.Add("THB");
                codes.Add("TRY");
                codes.Add("USD");
                codes.Add("ZAR");
            }
            return codes;
        }

        private void MapTimePeriods(string[] queryData, List<DateTime> startEndDates, string periodName)
        {
            DateTime outputDate;
            DateTime startEndDate;

            //path contains start or end date
            if (queryData.Any(item => item == periodName))
            {
                var periodIndex = Array.FindIndex(queryData, item => item == periodName);
                periodIndex++;

                var startEndDateStr = queryData[periodIndex];
                var dateParts = startEndDateStr.Split('-');
                startEndDate = new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]));


                if (periodName == "startPeriod")
                {
                    outputDate = startEndDate;
                }
                //Get or modify date for endDate
                else
                {
                    outputDate = GetModify(startEndDate);
                }

                startEndDates.Add(outputDate);
            }
            //Path doesn't contain startDate or endDate
            else
            {
                if (periodName == "startPeriod")
                {
                    //ECB beginning day for api data
                    outputDate = new DateTime(1999, 1, 4);
                }
                // if path has endDate but doesn't have "single record" constraint - assign Today's date to endDate
                else if (queryData.Any(item => item == "true"))
                {
                    outputDate = DateTime.Now;
                }
                else
                {
                    var startPeriodIndex = Array.FindIndex(queryData, item => item == "startPeriod");
                    startPeriodIndex++;

                    //CreateDate from index refactor -> repeated ^
                    var startEndDateStr = queryData[startPeriodIndex];
                    var dateParts = startEndDateStr.Split('-');
                    outputDate = new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]));
                }
                startEndDates.Add(outputDate);
            }
        }

        private DateTime GetModify(DateTime endDate)
        {
            DateTime output;
            // TODO: create extensions for some DateTime operations

            // check if endDate is today
            if (endDate.Day == DateTime.Now.Day)
            {
                //check if ECB data available
                if (DateTime.Now.Hour >= 16)
                {
                    output = endDate;
                }
                else
                {
                    var dayNum = DateTime.Now.Day;
                    dayNum--;
                    output = new DateTime(DateTime.Now.Year, DateTime.Now.Month, dayNum);
                }
            }
            // endDate is not today
            else
            {
                output = endDate;
            }
            return output;
        }

        private HttpRequestMessage CreateGetRequest(string endpoint)
        {
            var baseAddress = "https://sdw-wsrest.ecb.europa.eu/service/data/EXR/";
            var url = baseAddress + endpoint;
            var requestMsg = new HttpRequestMessage(HttpMethod.Get, url);
            requestMsg.Headers.Add("Accept", "application/json");
            requestMsg.Headers.Add("Accept-Encoding", "deflate");
            return requestMsg;
        }

        public void SetLastRequestEndpoint(string endpoint)
        {
            SingleLastRequest.Instance.Endpoint = endpoint;
        }
    }
}

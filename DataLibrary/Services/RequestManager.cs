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
        public string Endpoint { get; set; }
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
                HandleErrorCodes(response.StatusCode);
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        private void HandleErrorCodes(HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == HttpStatusCode.Unauthorized)
            {
                //TODO: open page showing error
            }
        }

        public List<string> MapEndpointToLookupKeys(string endpoint)
        {
            var queryData = endpoint.Split('.', '+', '?', '=', '&');
            List<string> lookupKeys;
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
            List<DateTime> allDates = new List<DateTime>();
            List<string> allDatesInStrings = new List<string>();

            var startDate = dates[0];
            var endDate = dates[1];

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    allDates.Add(date);
                }
            }
            foreach (var date in allDates)
            {
                var dateInString = date.ToString("yyyy_MM_dd");
                allDatesInStrings.Add(dateInString);
            }
            return allDatesInStrings;
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
            // refactor to get only codes data from ECB
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

        private void MapTimePeriods(string[] queryData, List<DateTime> dates, string periodName)
        {
            DateTime dateTime;
            if (queryData.Any(item => item == periodName))
            {
                var index = Array.FindIndex(queryData, item => item == periodName);
                index++;

                var date = queryData[index];
                var dateParts = date.Split('-');
                //var dateFormatted = dateParts[0] + "_" + dateParts[1] + "_" + dateParts[2];

                dateTime = new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]));
                dates.Add(dateTime);
            }
            else
            {
                //ECB beginning or last day for api data
                if (periodName == "startPeriod")
                {
                    dateTime = new DateTime(1999, 1, 4);
                }
                else
                {
                    if (DataForCurrentDayAvailable())
                    {
                        dateTime = DateTime.Now;
                    }
                    else
                    {
                        var now = DateTime.Now.Day;
                        now--;
                        dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, now);
                    }
                }
                dates.Add(dateTime);
            }
        }

        private bool DataForCurrentDayAvailable()
        {
            return DateTime.Now.Hour > 16;
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
    }
}

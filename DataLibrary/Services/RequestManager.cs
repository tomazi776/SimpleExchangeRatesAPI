using DataLibrary.Constants;
using DataLibrary.Extensions;
using DataLibrary.Helpers;
using DataLibrary.Models;
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
        private DateTime begginingECBDate = new DateTime(1999, 1, 4);

        public async Task<string> SendGetRequest(string endpoint)
        {
            HttpRequestMessage getRequest = CreateGetRequest(endpoint);
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                response = await client.SendAsync(getRequest);
            }

            //refactor to return status code
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode.ToString();
            }
            return await response.Content.ReadAsStringAsync();
        }


        public List<string> MapEndpointToLookupKeys(string endpoint)
        {
            List<string> lookupKeys;
            List<string> codes;
            List<DateTime> dates = new List<DateTime>();
            //List<string> query = endpoint.Split(EndpointHelper.separators).ToList();
            var query = endpoint.Split(EndpointHelper.separators);

            dates.Add(MapFrom(query, nameof(Period.startPeriod)));
            dates.Add(MapFrom(query, nameof(Period.endPeriod)));

            var workDates = DateTimeHelper.CreateWorkDatesFrom(dates);

            codes = MapCurrencyCodes(query);
            lookupKeys = CombineDatesWithCodes(workDates, codes);
            return lookupKeys;
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
            if (newCodes.Any() && newCodes.Count > 1)
            {
                foreach (var code in codes)
                {
                    onlyCodes += code + "+";
                }
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
                var keyData = key.Split(StringConstants.Underscore);
                var codePart = keyData[0];
                codes.Add(codePart);
            }
            return codes;
        }

        private string GetDatePart(string key)
        {
            var date = key.Split(StringConstants.Underscore).Last();
            return date;
        }

        private List<string> CombineDatesWithCodes(List<string> dates, List<string> codes)
        {
            List<string> lookupKeys = new List<string>();
            foreach (var date in dates)
            {
                foreach (var code in codes)
                {
                    var key = code + StringConstants.Underscore + date;
                    lookupKeys.Add(key);
                }
            }
            return lookupKeys;
        }

        private List<string> MapCurrencyCodes(string[] queryData)
        {
            List<string> codes = new List<string>();
            var codeIndex = Array.FindIndex(queryData, item => item == "SP00");
            codeIndex--;

            while (codeIndex > 1)
            {
                //skip last (denomination currCode)
                codeIndex--;
                codes.Add(queryData[codeIndex]);
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

        private DateTime MapFrom<T>(T query, string period) where T : IList<string>
        {
            DateTime date = query.Any(part => part == period)
                ? MapCustomStartEndDate(query, period)
                : AssignDefaultStartEndDate(query, period);
            return date;
        }

        private DateTime AssignDefaultStartEndDate<T>(T query, string period) where T : IList<string>
        {
            DateTime startEndDate;
            if (period == nameof(Period.startPeriod))
            {
                //ECB beginning date for api data
                startEndDate = begginingECBDate;
                return startEndDate;
            }

            return query.Any(item => item == "true")
                ? CreateDateFromQuery(query, "startPeriod")
                : CreateDateFromQuery(query, period);
        }

        private DateTime MapCustomStartEndDate<T>(T query, string period) where T : IList<string>
        {
            var date = CreateDateFromQuery(query, period);
            return date == DateTime.Today ? DateTimeHelper.GetLastAvailableECB(date) : date;
        }

        private DateTime CreateDateFromQuery<T>(T query, string period) where T : IList<string>
        {
            var partIndex = query.IndexOf(query.FirstOrDefault(part => part == period));
            var startEndDateStr = query[++partIndex];

            var dateParts = startEndDateStr.Split(StringConstants.EnDash);
            return new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]));
        }

        private HttpRequestMessage CreateGetRequest(string endpoint)
        {
            var url = BaseAddress + endpoint;
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

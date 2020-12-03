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

            var queryData = endpoint.Split('.', '+', '?', '=', '&');
            List<DateTime> outputDates = new List<DateTime>();
            List<string> codes;

            outputDates.Add(MapDate(queryData, "startPeriod"));
            outputDates.Add(MapDate(queryData, "endPeriod"));

            var formattedDates = DateTimeHelper.CreateRange(outputDates);

            codes = MapCurrencyCodes(queryData);
            lookupKeys = CombineDatesWithCodes(formattedDates, codes);
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
            if (newCodes.Count > 1)
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
                var keyData = key.Split('_');
                var codePart = keyData[0];
                codes.Add(codePart);
            }
            return codes;
        }

        private string GetDatePart(string key)
        {
            var keyData = key.Split('_');
            return keyData[1] + "-" + keyData[2] + "-"+ keyData[3];
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

        private DateTime MapDate(string[] queryData, string periodName)
        {
            DateTime outputDate = queryData.Any(item => item == periodName)
                ? MapCustomStartEndDate(queryData, periodName)
                : AssignDefaultStartEndDate(queryData, periodName);
            return outputDate;
        }

        private DateTime AssignDefaultStartEndDate(string[] queryData, string periodName)
        {
            DateTime startEndDate;
            if (periodName == "startPeriod")
            {
                //ECB beginning date for api data
                startEndDate = begginingECBDate;
                return startEndDate;
            }

            return queryData.Any(item => item == "true")
                ? CreateDateFromQuery(queryData, "startPeriod")
                : CreateDateFromQuery(queryData, periodName);
        }

        private DateTime MapCustomStartEndDate(string[] queryData, string periodName)
        {
            var startEndDate = CreateDateFromQuery(queryData, periodName);
            return periodName == "startPeriod" ? startEndDate : DateTimeHelper.GetModify(startEndDate);
        }

        private DateTime CreateDateFromQuery(string[] queryData, string periodName)
        {
            var periodIndex = Array.FindIndex(queryData, item => item == periodName);
            var startEndDateStr = queryData[++periodIndex];

            var dateParts = startEndDateStr.Split('-');
            return new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]));
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

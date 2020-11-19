using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Helpers
{
    public static class EndpointMapper
    {
        private const string defaultStartDate = "1991-01-04";
        private const string denominatedInEur = ".EUR";
        private const string referenceRatesCode = ".SP00";
        private const string variationMeasure = ".A";
        private const string optionalParamChar = "?";

        private const string dailyFrequency = "D.";
        private const string monthlyFrequency = "M.";
        public static string MapEndpoint(string currencyCodes, DateTime? startDate = null, bool single = false, DateTime? endDate = null)
        {
            string output = string.Empty;
            string start = startDate?.ToString("yyyy-MM-dd");
            string end = endDate?.ToString("yyyy-MM-dd");

            // TODO: change the rest to StringBuilder and refactor, add possibility to specify frequency
            StringBuilder sb = new StringBuilder();
            // if single == true get for single date where start == end
            if (single is true)
            {
                if (startDate != null)
                {
                    output = sb
                        .Append(dailyFrequency)
                        .Append(currencyCodes)
                        .Append(denominatedInEur)
                        .Append(referenceRatesCode)
                        .Append(variationMeasure)
                        .Append(optionalParamChar)
                        .Append("startPeriod=")
                        .Append(start)
                        .Append("&")
                        .Append("endPeriod=")
                        .Append(start).ToString();
                }
            }
            else
            {
                //set start date to default
                if (startDate is null)
                {
                    output = dailyFrequency + currencyCodes + denominatedInEur + referenceRatesCode + variationMeasure
                        + optionalParamChar + "startPeriod=" + defaultStartDate;
                }
                //set end date to now
                if (endDate is null)
                {
                    output = dailyFrequency + currencyCodes + denominatedInEur + referenceRatesCode + variationMeasure
                        + optionalParamChar + "startPeriod=" + start + "&" + "endPeriod=" + DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    output = dailyFrequency + currencyCodes + denominatedInEur + referenceRatesCode + variationMeasure
                        + optionalParamChar + "startPeriod=" + start + "&" + "endPeriod=" + end;
                }
            }
            return output;
        }
    }
}

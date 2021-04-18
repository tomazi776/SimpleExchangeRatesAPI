using DataLibrary.Constants;
using DataLibrary.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Helpers
{
    public static class InitialEndpointMapper
    {
        private const string defaultStartDate = EndpointHelper.defaultStartDate;
        private const string denominatedInEur = EndpointHelper.denominatedInEur;
        private const string referenceRatesCode = EndpointHelper.referenceRatesCode;
        private const string variationMeasure = EndpointHelper.variationMeasure;
        private const string optionalParamChar = EndpointHelper.optionalParamChar;
        private const string dailyFrequency = EndpointHelper.dailyFrequency;
        private const string monthlyFrequency = EndpointHelper.monthlyFrequency;

        //TODO: Refactor
        public static string MapEndpoint(string currencyCodes, DateTime? startDate = null, bool single = false, DateTime? endDate = null)
        {
            string endpoint = string.Empty;
            string start = startDate?.ToString(DateTimeHelper.YearMonthDayDashedFormat);
            string end = endDate?.ToString(DateTimeHelper.YearMonthDayDashedFormat);

            string modifiedStartDate = string.Empty;
            string modifiedEndDate = string.Empty;

            StringBuilder sb = new StringBuilder();

            //TODO: Refactor extract Method
            // care only for startDate as request doesn't specify range but single date
            if (single is true)
            {
                if (start != null)
                {
                    modifiedStartDate = start;
                    modifiedEndDate = start;
                }
            }

            //care for both as request specifies range on dates
            else
            {
                //set start date to default
                if (startDate is null)
                {
                    modifiedStartDate = defaultStartDate;
                }
                //set startDate to its value
                else
                {
                    modifiedStartDate = start;
                }

                //set end date to default (now)
                if (endDate is null)
                {
                    // Date dormat should remain dashed
                    modifiedEndDate = DateTime.Now.ToString(DateTimeHelper.YearMonthDayDashedFormat);
                }
                //set end date to its value
                else
                {
                    modifiedEndDate = end;
                }
            }

            endpoint = sb
                .Append(dailyFrequency)
                .Append(currencyCodes)
                .Append(denominatedInEur)
                .Append(referenceRatesCode)
                .Append(variationMeasure)
                .Append(optionalParamChar)
                .Append("startPeriod=")
                .Append(modifiedStartDate)
                .Append("&")
                .Append("endPeriod=")
                .Append(modifiedEndDate).ToString();
            return endpoint;
        }
    }
}

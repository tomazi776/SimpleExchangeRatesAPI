using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLibrary.Helpers
{
    public static class EndpointHelper
    {
        public const string defaultStartDate = "1999-01-04";
        public const string denominatedInEur = ".EUR";
        public const string referenceRatesCode = ".SP00";
        public const string variationMeasure = ".A";
        public const string optionalParamChar = "?";
        public const string startPeriod = "startPeriod";
        public const string endPeriod = "endPeriod";
        public static readonly char[] separators = new char[] { '.', '+', '?', '=', '&' };
        public const string dailyFrequency = "D.";
        public const string monthlyFrequency = "M.";
    }
}

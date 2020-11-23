using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLibrary.Helpers
{
    public static class EndpointHelper
    {
        private const string BaseAddress = "https://sdw-wsrest.ecb.europa.eu/service/data/EXR/";

        public const string defaultStartDate = "1999-01-04";
        public const string denominatedInEur = ".EUR";
        public const string referenceRatesCode = ".SP00";
        public const string variationMeasure = ".A";
        public const string optionalParamChar = "?";

        public const string dailyFrequency = "D.";
        public const string monthlyFrequency = "M.";
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader
{
    public class CurrencyConstraints : IRouteConstraint
    {
        private const char currencyDelimiter = '+';
        private Regex _firstCheckRegex;
        private Regex _secondCheckRegex;

        public CurrencyConstraints()
        {
            _firstCheckRegex = new Regex(@$"[{currencyDelimiter}]",
                                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
                                TimeSpan.FromMilliseconds(100));

            _secondCheckRegex = new Regex(@"[A-Z]{3}\W|[A-Z]{3}",
                                RegexOptions.CultureInvariant,
                                TimeSpan.FromMilliseconds(100));
        }
        public bool Match(HttpContext httpContext, IRouter route, string routeKey,
                          RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out object value))
            {
                var parameterValueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                if (parameterValueString == null || !_firstCheckRegex.IsMatch(parameterValueString))
                {
                    return false;
                }

                var splittedInput = parameterValueString.Split(currencyDelimiter);
                var matchCollection = _secondCheckRegex.Matches(parameterValueString);
                if (splittedInput.Count() != matchCollection.Count)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}

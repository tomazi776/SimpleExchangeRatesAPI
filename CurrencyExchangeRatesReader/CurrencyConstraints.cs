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
        private Regex _hasValidDelimiterRegex;
        private Regex _currencyCodesCheckRegex;

        public CurrencyConstraints()
        {
            _hasValidDelimiterRegex = new Regex(@$"[{currencyDelimiter}]",
                                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
                                TimeSpan.FromMilliseconds(100));

            _currencyCodesCheckRegex = new Regex(@"[A-Z]{3}\W|[A-Z]{3}",
                                RegexOptions.CultureInvariant,
                                TimeSpan.FromMilliseconds(100));
        }
        public bool Match(HttpContext httpContext, IRouter route, string routeKey,
                          RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out object value))
            {
                var parameter = Convert.ToString(value, CultureInfo.InvariantCulture);
                //TODO: support wildcarding
                if (parameter == null)
                {
                    return false;
                }
                // no currency code is valid (3 letter uppercase)
                if (!_currencyCodesCheckRegex.IsMatch(parameter))
                {
                    return false;
                }
                // is single match with 3 letter code, so check if further check required
                else
                {
                    // currCode consists of more than one so further check delimiter
                    if (parameter.Length > 3 && _hasValidDelimiterRegex.IsMatch(parameter))
                    {
                        var splittedInput = parameter.Split(currencyDelimiter);
                        var matchCollection = _currencyCodesCheckRegex.Matches(parameter);

                        if (splittedInput.Count() != matchCollection.Count)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}

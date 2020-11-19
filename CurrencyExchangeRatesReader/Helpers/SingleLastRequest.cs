using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Helpers
{
    public class SingleLastRequest
    {
        public string Endpoint { get; set; }
        public List<string> RequestParameters { get; set; }
        public TimeSpan LastRequestAbsoluteExpireTime { get; set; }


        private static readonly object padlock = new object();
        public static SingleLastRequest instance = null;
        private static SingleLastRequest Instance
        {
            get
            {
                lock (padlock)
                {
                    return instance ?? (instance = new SingleLastRequest());
                }
            }
        }
    }
}

using EasyCaching.Core;
using EasyCaching.Core.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Helpers
{
    public class CachingHelper
    {
        private IEasyCachingProviderFactory easyCachingProviderFactory;
        private IEasyCachingProvider easyCachingProvider;
        private readonly DateTime now;

        public CachingHelper(IEasyCachingProviderFactory easyCachingProviderFactory, IEasyCachingProvider easyCachingProvider)
        {
            this.easyCachingProviderFactory = easyCachingProviderFactory;
            this.easyCachingProvider = this.easyCachingProviderFactory.GetCachingProvider("redis1");
            this.now = DateTime.Now;
        }


        public void CacheLocally()
        {

            //if (NotInCache(data))
            //{
            //    AddToCache(data);
            //}
            //else
            //{

            //}


            //easyCachingProvider.Set(string)
        }

        //public decimal GetCurrencyRateFor(string desiredCurrency, DateTime? dateTime = null)
        //{

        //}
    }
}

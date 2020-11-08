using DataLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DataLibrary
{
    public class CurrencyRepository : IRepository
    {
        //private IConnectionProvider _connProvider;

        public List<T> GetCurrenciesER<T>(Dictionary<string, T> keyValuePairs)
        {
            throw new NotImplementedException();
        }

        public T GetSingleCurrencyER<T>(Expression<Func<string, string>> expression)
        {
            throw new NotImplementedException();
        }

        public int SaveRates<T>()
        {
            throw new NotImplementedException();
        }
    }
}

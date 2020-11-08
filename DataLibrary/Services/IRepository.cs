using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataLibrary.Services
{
    public interface IRepository
    {
        T GetSingleCurrencyER<T>(Expression<Func<string, string>> expression);
        List<T> GetCurrenciesER<T>(Dictionary<string, T> keyValuePairs);
        int SaveRates<T>();
    }
}
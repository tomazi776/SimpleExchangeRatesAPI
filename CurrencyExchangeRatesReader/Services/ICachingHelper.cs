using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Services
{
    public interface ICachingHelper
    {
        Task<List<ICurrencyModel>> MapRequestData(ICurrencyModel model);
    }
}

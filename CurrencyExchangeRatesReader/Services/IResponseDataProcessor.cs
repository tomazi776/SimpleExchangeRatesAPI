using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Services
{
    public interface IResponseDataProcessor
    {
        void HandleErrorCodes();
        List<ICurrencyModel> Deserialize(ICurrencyModel model, string json);
    }
}

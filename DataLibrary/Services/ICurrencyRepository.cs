using DataLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public interface ICurrencyRepository
    {
        //T GetSingleCurrencyER<T>(Expression<Func<string, string>> expression);
        //List<T> GetCurrenciesER<T>(Dictionary<string, T> keyValuePairs) where T : ICurrencyModel;
        Task<IList<string>> GetData(ICurrencyModel model, string requestUrl);
        int SaveData<T>();

    }
}
using DataLibrary.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public interface ICurrencyRepository
    {
        Task<IList<string>> GetData(ICurrencyModel model, string endpoint);
    }
}
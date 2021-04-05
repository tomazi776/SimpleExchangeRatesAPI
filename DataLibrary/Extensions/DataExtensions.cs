using DataLibrary.Constants;
using DataLibrary.Models;

namespace DataLibrary.Extensions
{
    public static class DataExtensions
    {
        public static string CreateId<T>(this T data) where T : ICurrencyModel
        {
            return data.Code + StringConstants.Underscore + data.ObservationDate;
        }
    }
}

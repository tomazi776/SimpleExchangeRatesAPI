using DataLibrary.Models;

namespace DataLibrary.Extensions
{
    public static class DataExtensions
    {
        public static string CreateId<T>(this T data) where T : ICurrencyModel
        {
            var formattedDate = data.ObservationDate.ToString("yyyy_MM_dd");
            string id = data.Code + formattedDate;
            return id;
        }
    }
}

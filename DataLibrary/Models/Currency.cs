using CurrencyExchangeRatesReader.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DataLibrary.Models
{
    [Serializable]
    public class Currency : ICurrencyModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime ObservationDate { get; set; }
    }
}

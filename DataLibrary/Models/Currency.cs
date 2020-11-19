using System;

namespace DataLibrary.Models
{
    public class Currency : ICurrencyModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal ExchangeRate { get; set; }
        public string ObservationDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class Currency : ICurrencyModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime ObservationDate { get; set; }
    }
}

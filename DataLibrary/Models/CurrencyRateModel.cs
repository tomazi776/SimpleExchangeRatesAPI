using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class CurrencyRateModel
    {
        public string Code { get; set; }
        public double ExchangeRate { get; set; }
        public List<DateTime> DateRange { get; set; }
    }
}

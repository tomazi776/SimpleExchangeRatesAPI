using System;

namespace DataLibrary.Models
{
    public interface ICurrencyModel
    {
        string Code { get; set; }
        string Name { get; set; }
        decimal ExchangeRate { get; set; }
        string ObservationDate { get; set; }
    }
}
using System;

namespace DataLibrary.Models
{
    public interface ICurrencyModel : IObservation
    {
        string Code { get; set; }
        string Name { get; set; }
        decimal ExchangeRate { get; set; }
    }

    public interface IObservation
    {
        DateTime ObservationDate { get; set; }
    }
}
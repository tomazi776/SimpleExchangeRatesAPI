using CurrencyExchangeRatesReader.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.JsonModels
{
    [Newtonsoft.Json.JsonConverter(typeof(ApiCurrencyNameConverter))]
    public class RootObject
    {
        [Newtonsoft.Json.JsonIgnore]
        [JsonPropertyName("header")]
        public Header Header { get; set; }

        [JsonProperty]
        [JsonPropertyName("dataSets")]
        public List<DataSet> DataSets { get; set; }

        [JsonProperty]
        [JsonPropertyName("structure")]
        public Structure Structure { get; set; }
    }

    public class Header
    {
    }

    [JsonArray]
    public class DataSet
    {
        public Series series { get; set; }
    }

    public class Series
    {
        [JsonProperty]
        public Currency Currency { get; set; }
    }

    public class Currency
    {
        public Observation observations { get; set; }
    }

    public class Observation
    {
        [JsonProperty]
        public TimeFrame TimeFrame { get; set; }
    }

    [JsonArray]
    public class TimeFrame
    {
        [JsonProperty]
        public decimal ExchangeRate { get; set; }
    }

    public class Structure
    {
    }
}

using System.Net.Http;
using System.Net.Http.Headers;

namespace CurrencyExchangeRatesReader.Services
{
    interface IRequestProvider
    {
        public string RequestUrl { get; set; }
        public HttpRequestMessage RequestMessage { get; set; }
        public HttpRequestHeaders RequestHeaders { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }

    }
}
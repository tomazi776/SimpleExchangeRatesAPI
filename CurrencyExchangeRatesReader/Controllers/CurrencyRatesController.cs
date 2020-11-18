using DataLibrary.Models;
using DataLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;

namespace CurrencyExchangeRatesReader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CurrencyRatesController : ControllerBase
    {

        private readonly ILogger<CurrencyRatesController> _logger;
        private readonly ICurrencyModel _currencyModel;
        private readonly ICurrencyRepository _currencyRepository;
        private const string PLN_USD_EUR_FromTenthNovQuery = "D.PLN+USD.EUR.SP00.A?startPeriod=2020-11-10&detail=dataonly";

        public CurrencyRatesController(ICurrencyModel model, ILogger<CurrencyRatesController> logger, ICurrencyRepository currencyRepository)
        {
            //TODO:
            //Add Run Redis server if not running already
            //ServiceController.GetServices - smth like that
            _currencyModel = model;
            _logger = logger;
            _currencyRepository = currencyRepository;
        }


        /// <summary>
        /// Gets exchange rates for PLN and USD denominated in EUR starting from 10th of November
        /// </summary>
        /// <returns></returns>
        [ApiKeyAuth]
        [HttpGet("Get/PLN_USD_EUR")]
        public IEnumerable<JsonDocument> GetExchangeRatesForPLNAndUSD([FromHeader]string apiKey)
        {
            //AddLookupKeysFromEndpoint(endpoint);
            List<JsonDocument> jsonObjects = new List<JsonDocument>();
            var currencyData = _currencyRepository.GetData(_currencyModel, PLN_USD_EUR_FromTenthNovQuery).Result;

            foreach (var item in currencyData)
            {
                var jsonData = JsonDocument.Parse(item);
                jsonObjects.Add(jsonData);
            }
            return jsonObjects;
        }


        //private void AddLookupKeysFromEndpoint(string endpoint)
        //{
        //    var lookupKeys = _requestManager.MapEndpointToLookupKeys(endpoint);
        //    foreach (var key in lookupKeys)
        //    {
        //        _cachingHelper.KeysToLookup.Add(key);
        //    }
        //}
    }
}

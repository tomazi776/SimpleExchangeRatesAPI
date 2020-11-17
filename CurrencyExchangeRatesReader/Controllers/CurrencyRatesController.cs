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
        private const string PLN_EUR_FromFourthNovQuery = "D.PLN+USD.EUR.SP00.A?startPeriod=2020-11-10&detail=dataonly";

        public CurrencyRatesController(ICurrencyModel model, ILogger<CurrencyRatesController> logger, ICurrencyRepository currencyRepository)
        {
            //TODO:
            //Add Run Redis server if not running already
            //ServiceController.GetServices - smth like that
            _currencyModel = model;
            _logger = logger;
            _currencyRepository = currencyRepository;
        }

        [HttpGet("Get/PLN_EUR_FromFourthNov")]
        public IEnumerable<JsonDocument> GetExchangeRatesForPLN()
        {
            //AddLookupKeysFromEndpoint(endpoint);
            List<JsonDocument> jsonObjects = new List<JsonDocument>();
            var currencyData = _currencyRepository.GetData(_currencyModel, PLN_EUR_FromFourthNovQuery).Result;

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

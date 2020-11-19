using CurrencyExchangeRatesReader.Helpers;
using DataLibrary.Models;
using DataLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        /// Gets exchange rates for given currencies denominated in EUR starting from start
        /// </summary>
        /// <returns> A collection of currency exchange rates for given date ranges</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET PLN/EUR
        ///     {
        ///         "rootElement": 
        ///         {
        ///           "Code": "PLN",
        ///           "Name": "Polish zloty",
        ///           "ExchangeRate": 4.4692
        ///           "ObservationDate" 2020-11-13
        ///         }
        ///     }
        /// </remarks>
        /// <response code="200">Returns currencies with exchange rate info</response>
        /// <response code="400">If the item is null</response> 
        /// <response code="401">If the supplied APIKey in header is not valid</response> 
        /// <response code="404">If exchange rate not found for date in given date range or for invalid endpoint</response>
        [ApiKeyAuth]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [Produces("application/json")]
        [HttpGet("Get/{currencyCodes:codesConstraint}/{startDate:datetime?}/{single:bool?}/{endDate:datetime?}")]
        public IEnumerable<JsonDocument> GetExchangeRatesForPLNAndUSD([FromHeader]string apiKey, 
            string currencyCodes, DateTime? startDate = null, bool single = false, DateTime? endDate = null)
        {
            var endpoint = EndpointMapper.MapEndpoint(currencyCodes, startDate, single, endDate);
            var currencyData = _currencyRepository.GetData(_currencyModel, endpoint).Result;
            List<JsonDocument> jsonObjects = new List<JsonDocument>();

            foreach (var item in currencyData)
            {
                var jsonData = JsonDocument.Parse(item);
                jsonObjects.Add(jsonData);
            }
            return jsonObjects;
        }
    }
}

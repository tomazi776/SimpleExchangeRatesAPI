using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CurrencyExchangeRatesReader.Helpers;
using CurrencyExchangeRatesReader.Services;
using DataLibrary.Models;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeRatesReader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CurrencyRatesController : ControllerBase
    {
        //private IEasyCachingProviderFactory _cachingProviderFactory;
        //private IEasyCachingProvider _cachingProvider;
        private readonly ILogger<CurrencyRatesController> _logger;
        private ICachingHelper _cachingHelper;
        private ICurrencyModel _model;


        public CurrencyRatesController(ICachingHelper cachingHelper, ICurrencyModel model, ILogger<CurrencyRatesController> logger)
        {
            //Add Run Redis server if not running already
            //ServiceController.GetServices - smth like that
            _cachingHelper = cachingHelper;
            _model = model;
            _logger = logger;
        }

        [HttpGet("Get/PLN_EUR_FromFourthNov")]
        public void GetExchangeRatesForPLN()
        {
            //var newCurrRateObj = new CurrencyRateModel();
            var data = _cachingHelper.MapRequestData(_model);

            //Setting expiration date should be dependant on date - longer for monthly data, short for days
            //_cachingProvider.Set("D_PLN_EUR_Start_2020-11-04", data, TimeSpan.FromMinutes(60));
        }

        // GET: api/<CurrencyRatesController>
        [HttpGet("Get")]
        public IEnumerable<string> GetAllRates()
        {
            return new string[] { "value1", "value2" };
        }
    }
}

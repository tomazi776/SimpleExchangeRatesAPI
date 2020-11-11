using System.Collections.Generic;
using DataLibrary.Models;
using DataLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeRatesReader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CurrencyRatesController : ControllerBase
    {

        private readonly ILogger<CurrencyRatesController> _logger;
        private readonly ICurrencyModel _currencyModel;
        private readonly IDistributedCache _distributedCache;
        private readonly ICurrencyRepository _currencyRepository;
        private const string baseServiceAddress = "https://sdw-wsrest.ecb.europa.eu/service/data/EXR/";
        private const string pLN_EUR_FromFourthNovQuery = "D.PLN+USD.EUR.SP00.A?startPeriod=2020-11-05&detail=dataonly";

        public CurrencyRatesController(ICurrencyModel model,
            IDistributedCache distributedCache, ILogger<CurrencyRatesController> logger, ICurrencyRepository currencyRepository)
        {
            //Add Run Redis server if not running already
            //ServiceController.GetServices - smth like that
            _currencyModel = model;
            _logger = logger;
            _distributedCache = distributedCache;
            _currencyRepository = currencyRepository;
        }

        [HttpGet("Get/PLN_EUR_FromFourthNov")]
        public void GetExchangeRatesForPLN()
        {
            var currencyData = _currencyRepository.GetData(_currencyModel);

            //foreach (var currency in currencyData)
            //{
            //    var id = currency.CreateId();

            //    //TODO: make timespan depend upon data calling frequency
            //}

        }

        //public void OnRequestBtnClicked()
        //{
        //    RequestMapper mapper = new RequestMapper();
        //    mapper.Query = "QueryFrom clicked button";
        //}

        // GET: api/<CurrencyRatesController>
        [HttpGet("Get")]
        public IEnumerable<string> GetAllRates()
        {
            return new string[] { "value1", "value2" };
        }
    }
}

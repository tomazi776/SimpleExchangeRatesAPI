using DataLibrary.Models;
using DataLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeRatesReader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CurrencyRatesController : ControllerBase
    {

        private readonly ILogger<CurrencyRatesController> _logger;
        private readonly ICurrencyModel _currencyModel;
        private readonly ICurrencyRepository _currencyRepository;
        private const string PLN_EUR_FromFourthNovQuery = "D.PLN+USD.EUR.SP00.A?startPeriod=2020-11-05&detail=dataonly";

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
        public void GetExchangeRatesForPLN()
        {
            var currencyData = _currencyRepository.GetData(_currencyModel, PLN_EUR_FromFourthNovQuery);
        }
    }
}

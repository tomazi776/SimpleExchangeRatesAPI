using DataLibrary.Services;
using Microsoft.AspNetCore.Mvc;


namespace CurrencyExchangeRatesReader.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientAuthController : ControllerBase
    {
        private readonly IApiKeyManager _apiKeyGenerator;
        public ClientAuthController(IApiKeyManager apiKeyGenerator)
        {
            _apiKeyGenerator = apiKeyGenerator;
        }
        
        /// <summary>
        /// Generates a unique API Key used for client request authorization
        /// </summary>
        /// <returns></returns>
        [HttpGet("Get/Key")]
        public string GetApiKey()
        {
            return _apiKeyGenerator.GenerateKey();
        }
    }
}

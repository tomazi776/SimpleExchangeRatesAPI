using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public class RequestManager : IRequestManager
    {
        public void HandleErrorCodes()
        {
            throw new NotImplementedException();
        }

        public async Task<string> SendRequest()
        {
            HttpResponseMessage response;
            HttpRequestMessage request = CreateRequest("https://sdw-wsrest.ecb.europa.eu/service/data/EXR/D.PLN+USD.EUR.SP00.A?startPeriod=2020-11-05&detail=dataonly");
            using (var client = new HttpClient())
            {
                response = await client.SendAsync(request);
            }

            //Handle if not
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        private HttpRequestMessage CreateRequest(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("Accept", "application/json");
            req.Headers.Add("Accept-Encoding", "deflate");
            return req;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public interface IRequestManager
    {
        List<string> MapEndpointToLookupKeys(string endpoint);
        Task<string> SendGetRequest(string requestUrl);
        public string Endpoint { get; set; }
    }
}

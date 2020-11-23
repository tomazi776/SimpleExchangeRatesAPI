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
        void SetLastRequestEndpoint(string endpoint);
        string CreateEndpointForMissingTimeFrame(HashSet<string> notFoundKeys);
        
    }
}

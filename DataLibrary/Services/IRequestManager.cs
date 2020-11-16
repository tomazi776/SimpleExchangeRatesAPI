using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public interface IRequestManager
    {
        void HandleErrorCodes();
        Task<string> SendRequest();
    }
}

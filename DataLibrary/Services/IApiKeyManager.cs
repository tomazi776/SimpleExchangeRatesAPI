using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Services
{
    public interface IApiKeyManager
    {
        string GenerateKey();
        bool Matches(string potentialKey);
    }
}

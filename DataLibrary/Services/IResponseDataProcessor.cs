using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Services
{
    public interface IResponseDataProcessor
    {
        List<ICurrencyModel> Deserialize(ICurrencyModel model, string json);
    }
}

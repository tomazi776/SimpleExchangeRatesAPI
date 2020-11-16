﻿using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public interface ICachingHelper
    {
        public List<string> KeysToLookup { get; set; }
        Task SaveDataToCache(List<ICurrencyModel> data);
        Task<List<string>> LoadDataFromCache(ICurrencyModel data);
    }
}
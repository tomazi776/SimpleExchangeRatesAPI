using DataLibrary.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Dapper;

namespace DataLibrary
{
    public class ApiKeyManager : IApiKeyManager
    {
        private readonly IConfiguration _config;

        public ApiKeyManager(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string GenerateKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            string apiKey = Convert.ToBase64String(key);
            Save(apiKey);
            return apiKey;
        }
        private void Save(string key)
        {
            var cnnStr = _config.GetSection("MyConnStr").Value;
            using (IDbConnection conn = new SqlConnection(cnnStr))
            {
                string query = @$"INSERT INTO [ApiKey] (ApiKeyValue) values ('{key}');";
                conn.Execute(query);
            }
        }

        public bool Matches(string potentialKey)
        {
            var cnnStr = _config.GetSection("MyConnStr").Value;
            using (IDbConnection conn = new SqlConnection(cnnStr))
            {
                var query = @$"SELECT ApiKeyValue FROM [ApiKey] WHERE ApiKeyValue = '{potentialKey}';";
                var result = conn.Query<string>(query);
                return result.AsList().Count == 1;
            }
        }
    }
}

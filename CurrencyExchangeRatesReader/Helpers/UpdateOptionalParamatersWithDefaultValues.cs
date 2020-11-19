using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRatesReader.Helpers
{
    public class UpdateOptionalParamatersWithDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

            if (descriptor != null && descriptor.ControllerName.StartsWith("Currency"))
            {
                operation.Parameters.Clear();
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "currencyCodes",
                    In = ParameterLocation.Query,
                    Required = true
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "startDate",
                    In = ParameterLocation.Query,
                    Required = false
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "single",
                    In = ParameterLocation.Query,
                    Required = false
                });

                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "endDate",
                    In = ParameterLocation.Query,
                    Required = false
                });
            }
        }
    }
}

using CurrencyExchangeRatesReader.Helpers;
using CurrencyExchangeRatesReader.Services;
using DataLibrary;
using DataLibrary.Models;
using DataLibrary.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CurrencyExchangeRatesReader
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                //Ensure key's uniqueness per app domain
                //options.InstanceName = "ERApi_";
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "Currency Exchange API",
                    Description = "An efficient API for displaying currency exchange rates from European Central Bank",
                    Version = "V1"
                });
            });

            //TODO: Ensure proper scoping
            services.AddSingleton<ICurrencyRepository, CurrencyRepository>();
            services.AddSingleton<IRequestManager, RequestManager>();
            services.AddSingleton<IResponseDataProcessor, ResponseDataProcessor>();
            services.AddSingleton<ICachingHelper, CachingHelper>();
            services.AddScoped<ICurrencyModel, Currency>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Demo API");
            });
        }
    }
}

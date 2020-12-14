using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using AutoMapper;
using BlazorTable;
using Eveindustry.Shared;
using Eveindustry.Shared.Profiles;
using EveIndustry.Web.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EveIndustry.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
#if DEBUG
            var baseAddress = "https://localhost:1337";
#else
            var baseAddress = builder.HostEnvironment.BaseAddress;
#endif

            builder.Services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(baseAddress)})
                .AddAutoMapper(c => c.AddProfile<EveItemManufacturingInfoMappingProfile>())
                .AddBlazorTable()
                
                .AddScoped<IEveItemSearchService, EveItemSearchService>()
                .AddScoped<IManufacturingInfoBuilder, ManufacturingInfoBuilder>()
                ;

            await builder.Build().RunAsync();
        }
    }
}
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Eveindustry.API.Config;
using Eveindustry.Core;
using Eveindustry.Core.Models;
using Eveindustry.Core.Models.Config;
using Eveindustry.Sde;
using Eveindustry.Sde.Models.Config;
using Eveindustry.Sde.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
namespace Eveindustry.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public ILifetimeScope Container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<AppConfiguration>();
            services.AddOptions<TypeInfoLoaderOptions>().Bind(Configuration.GetSection(nameof(AppConfiguration.TypeInfoLoaderOptions)));
            services.AddOptions<EvePricesUdateConfiguration>().Bind(Configuration.GetSection(nameof(AppConfiguration.EvePricesUdateConfiguration)));
            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Eveindustry.API", Version = "v1"});
            });
            services.AddSingleton<ISdeDataRepository, SdeDataRepository>();
            services.AddSingleton<ISdeDataLoader, SdeDataLoader>();
            services.AddSingleton<IEsiPricesRepository, EsiPricesRepository>();
            services.AddSingleton<IEvePricesRepository, EvePricesRepository>();
            services.AddSingleton<ITypeIdsSource, AllTypeIdsSource>();
            
            services.AddSingleton<IEveTypeRepository, EveTypeRepository>();
            
            services.AddAutoMapper(typeof(Startup).Assembly, typeof(EveType).Assembly);
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.Container = app.ApplicationServices.GetAutofacRoot();
            this.InitializeServices();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eveindustry.API v1"));
                app.UseCors(c =>
                {
                    c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseBlazorFrameworkFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
            
        }

        public void InitializeServices()
        {
             var logger = Container.Resolve<ILogger<Startup>>();
             logger.LogInformation("Starting to initialize repositories");
             logger.LogInformation("Begin initialize ISdeDataRepository");
             Container.Resolve<ISdeDataRepository>().Init().Wait();
             logger.LogInformation("End initialize ISdeDataRepository");
             logger.LogInformation("Begin initialize IEsiPricesRepository");
             Container.Resolve<IEsiPricesRepository>().Init().Wait();
            logger.LogInformation("End initialize IEsiPricesRepository");
            logger.LogInformation("Begin initialize IEvePricesRepository");
            Container.Resolve<IEvePricesRepository>().Init().Wait();
            logger.LogInformation("End initialize IEvePricesRepository");
            logger.LogInformation("Begin initialize IEveTypeRepository");
            Container.Resolve<IEveTypeRepository>().Init().Wait();
            logger.LogInformation("End initialize IEveTypeRepository");
        }
    }
}
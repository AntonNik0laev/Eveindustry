using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Eveindustry.Sde;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveIndustry.UpdateData
{
    public class UpdateDataHostedService: IHostedService
    {
        private readonly ILogger<UpdateDataHostedService> logger;
        private readonly ISdeDataLoader loader;

        public UpdateDataHostedService(ILogger<UpdateDataHostedService> logger, ISdeDataLoader loader)
        {
            this.logger = logger;
            this.loader = loader;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var binPDir = AppDomain.CurrentDomain.BaseDirectory;

            var sdeFiles = Directory.GetFiles(binPDir, "*.bin");
            foreach (var file in sdeFiles)
            {
                this.logger.LogInformation($"removing binary file: {file}");
                File.Delete(file);
            }

            this.loader.Load().Wait();

            if (File.Exists(Path.Combine(binPDir, "sdedata.bin")))
            {
                logger.LogInformation("sdedata.bin created");
                Environment.Exit(0);
                return Task.CompletedTask;
                ;
            }
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
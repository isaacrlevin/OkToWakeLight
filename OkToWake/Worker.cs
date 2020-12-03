using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OkToWake.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OkToWake
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceProvider services,
            ILogger<Worker> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RunScopedWorker(stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task RunScopedWorker(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IBackgroundScheduleProcessor>();

                await scopedProcessingService.ProcessSchedules();
            }
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
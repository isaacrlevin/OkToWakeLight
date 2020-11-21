using System;
using System.Threading;
using System.Threading.Tasks;
using LifxCloud.NET.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OkToWake
{
    public class Worker : BackgroundService
    {
        private readonly ConfigWrapper Config;
        private readonly AppState _appState;
        private readonly ILogger<Worker> _logger;


        private LIFXService _lifxService;
        public Worker(ILogger<Worker> logger,
                      IOptionsMonitor<ConfigWrapper> optionsAccessor,
                      AppState appState,
                      LIFXService lifxService)
        {
            Config = optionsAccessor.CurrentValue;
            _lifxService = lifxService;
            _logger = logger;
            _appState = appState;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await GetData();                   
                }
                catch (Exception e)
                {
                    var foo = e;
                }
                await Task.Delay(Convert.ToInt32(Config.PollingInterval * 1000), stoppingToken);
            }
        }


        private async Task GetData()
        {
            if (!string.IsNullOrEmpty(Config.LIFXApiKey))
            {
                foreach (TimeInterval interval in Config.TimeIntervals)
                {
                    if (IsInRange(interval.TimeIntervalStart, interval.TimeIntervalEnd))
                    {
                        await _lifxService.SetColor(interval.TimeColor, (Selector)Config.SelectedLIFXItemId);
                        _logger.LogInformation($"Current time is : {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}, interval: {interval.TimeIntervalName}, range: {interval.TimeIntervalStart} - {interval.TimeIntervalEnd}, light updated to {interval.TimeColor}");
                        break;
                    }
                }
            }

            Thread.Sleep(Convert.ToInt32(Config.PollingInterval * 1000));
        }

        private bool IsInRange(string startTime, string endTime)
        {
            bool isInRange = false;
            // convert datetime to a TimeSpan
            bool validStart = TimeSpan.TryParse(startTime, out TimeSpan start);
            bool validEnd = TimeSpan.TryParse(endTime, out TimeSpan end);

            TimeSpan now = DateTime.Now.TimeOfDay;
            // see if start comes before end
            if (start < end)
            {
                isInRange = start <= now && now <= end;
                return isInRange;
            }
            // start is after end, so do the inverse comparison

            isInRange = !(end < now && now < start);

            return isInRange;
        }
    }
}

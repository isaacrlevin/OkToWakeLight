using LifxCloud.NET;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LifxCloud.NET.Models;
using Microsoft.Extensions.Configuration;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var _client = await LifxCloudClient.CreateAsync(_config["LIFXAPiKey"]);
            var lights = await _client.ListLights();
            Selector selector = (Selector)_config["LIFXLightId"];

            while (!stoppingToken.IsCancellationRequested)
            {
                string color = "";
                if (IsInRange(_config["AsleepTimeSpan"]))
                {
                    color = _config["AsleepLightColor"];
                }

                if (IsInRange(_config["WakeupTimeSpan"]))
                {
                    color = _config["WakeupLightColor"];
                }

                if (IsInRange(_config["AwakeTimeSpan"]))
                {
                    color = _config["AwakeLightColor"];
                }

                if (IsInRange(_config["BedTimeTimeSpan"]))
                {
                    color = _config["BedTimeLightColor"];
                }

                var result = await _client.SetState(selector, new LifxCloud.NET.Models.SetStateRequest
                {
                    Color = color,
                    Duration = 0,
                    Brightness = Convert.ToDouble(_config["Brightness"])
                });

                await Task.Delay(60000, stoppingToken);
            }
        }
        bool IsInRange(string timeRangeString)
        {
            string[] timeRange = timeRangeString.Split("-");
            string startTime = timeRange[0];
            string endTime = timeRange[1];
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

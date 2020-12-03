using System;
using LifxCloud.NET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OkToWake.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace OkToWake.Services
{
    internal interface IBackgroundScheduleProcessor
    {
        Task ProcessSchedules();
    }

    internal class BackgroundScheduleProcessor : IBackgroundScheduleProcessor
    {
        private readonly ILogger<BackgroundScheduleProcessor> _logger;
        private readonly OkToWakeContext _context;

        public BackgroundScheduleProcessor(ILogger<BackgroundScheduleProcessor> logger,
            OkToWakeContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ProcessSchedules()
        {
            var schedules = await _context.Schedules.Include(a=> a.TimeIntervals).ToListAsync();

            var tasks = schedules.Select(async schedule => {
                if (schedule.IsEnabled)
                {
                    if (string.IsNullOrEmpty(schedule.LIFXApiKey))
                    {
                        _logger.LogInformation($"LIFXKEY Not Set");
                        return;
                    }

                    if (string.IsNullOrEmpty(schedule.SelectedLIFXItemId))
                    {
                        _logger.LogInformation($"No LIFX Item specified");
                        return;
                    }

                    _logger.LogInformation($"Using LIFXKEY: {schedule.LIFXApiKey}");
                    try
                    {
                        if (schedules != null && schedules.Count > 0)
                        {
                            while (true)
                            {
                                if (await UpdateLights(schedule))
                                    break;
                                await Task.Delay(Convert.ToInt32(schedule.PollingInterval * 1000));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }
            }).ToArray();

            await Task.WhenAll(tasks);
        }


        private async Task<bool> UpdateLights(Schedule schedule)
        {
            var _lifxService = new LIFXService(schedule.LIFXApiKey);
            try
            {
                var lights = await _lifxService.GetAllLightsAsync();
                var groups = await _lifxService.GetAllGroupsAsync();

                if ((groups == null || groups.Count == 0) &&
                    (lights == null || lights.Count == 0))
                {
                    _logger.LogInformation($"No lights configured for LIFX");
                    return true;
                }

                foreach (var light in lights)
                {
                    if ($"id:{light.Id}" == schedule.SelectedLIFXItemId)
                    {
                        _logger.LogInformation($"Ok to Wake {schedule.ScheduleName} Linked to {light.Label}");
                    }
                }

                foreach (var group in groups)
                {
                    if ($"group_id:{group.Id}" == schedule.SelectedLIFXItemId)
                    {
                        _logger.LogInformation($"Ok to Wake Linked to {group.Label} Group");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            try
            {
                foreach (TimeInterval interval in schedule.TimeIntervals)
                {
                    if (IsInRange(interval.TimeIntervalStartAsDate.TimeOfDay.ToString(), interval.TimeIntervalEndAsDate.TimeOfDay.ToString()))
                    {
                        await _lifxService.SetColor(interval.TimeColor, schedule.Brightness, (Selector)schedule.SelectedLIFXItemId);
                        _logger.LogInformation($"Scedule: {schedule.ScheduleName} Current time is : {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}, interval: {interval.TimeIntervalName}, range: {interval.TimeIntervalStartAsDate.TimeOfDay.ToString()} - {interval.TimeIntervalEndAsDate.TimeOfDay.ToString()}, light updated to {interval.TimeColor}");
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return true;
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OkToWake.Models
{
    public class Schedule
    {
        public Guid ScheduleId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Schedule name cannot exceed 100 characters.")]
        public string ScheduleName { get; set; }
        public string LIFXApiKey { get; set; }
        public string SelectedLIFXItemId { get; set; }
        public int Brightness { get; set; } = 100;
        public double PollingInterval { get; set; } = 5.0;
        public bool IsEnabled { get; set; } = false;
        public List<TimeInterval> TimeIntervals { get; set; } = new List<TimeInterval>();
    }
}

using System;
using System.ComponentModel.DataAnnotations;
namespace OkToWake.Models
{
    public class TimeInterval
    {
        public Guid TimeIntervalId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Interval name cannot exceed 100 characters.")]
        public string TimeIntervalName { get; set; }

        public string TimeColor { get; set; }

        [DataType(DataType.Date)]
        public DateTime TimeIntervalStartAsDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime TimeIntervalEndAsDate { get; set; }

        public bool IsEnabled { get; set; } = false;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace OkToWake
{
    public class ConfigWrapper
    {
        public string LIFXApiKey { get; set; }
        public string SelectedLIFXItemId { get; set; }
        public int Brightness { get; set; }
        public double PollingInterval { get; set; }
        public List<TimeInterval> TimeIntervals { get; set; }
    }

    public class TimeInterval
    {
        public string TimeIntervalName { get; set; }

        public string TimeColor { get; set; }

        public string TimeIntervalStart { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [JsonProperty(Required = Required.Default)]
        public DateTime? TimeIntervalStartAsDate { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [JsonProperty(Required = Required.Default)]
        public DateTime? TimeIntervalEndAsDate { get; set; }

        public string TimeIntervalEnd { get; set; }         
    }
}
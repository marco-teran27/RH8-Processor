using System.Text.Json.Serialization;
using Commons.Interfaces;
using Commons.Utils;

namespace Config.Models
{
    public class TimeOutSettings : ITimeOutSettings
    {
        [JsonPropertyName("minutes")]
        [JsonConverter(typeof(TimeOutSettingsConverter))] // Added
        public int Minutes { get; set; } = 8;
    }
}
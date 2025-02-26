using System.Text.Json.Serialization;
using Commons.Interfaces;

namespace Config.Models
{
    public class TimeOutSettings : ITimeOutSettings
    {
        [JsonPropertyName("minutes")]
        public int Minutes { get; set; } = 8;
    }
}
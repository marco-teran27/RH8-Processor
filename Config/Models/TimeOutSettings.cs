using System.Text.Json.Serialization;

namespace Config.Models
{
    public class TimeOutSettings
    {
        [JsonPropertyName("minutes")]
        public int Minutes { get; set; }
    }
}
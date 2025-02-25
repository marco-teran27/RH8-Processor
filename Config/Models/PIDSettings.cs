using System.Text.Json.Serialization;

namespace Config.Models
{
    public class PIDSettings
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("pids")]
        public string[] Pids { get; set; }
    }
}
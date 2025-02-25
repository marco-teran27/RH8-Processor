using System.Text.Json.Serialization;

namespace Config.Models
{
    public class PIDSettings
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = string.Empty;

        [JsonPropertyName("pids")]
        public List<string> Pids { get; set; } = new();
    }
}
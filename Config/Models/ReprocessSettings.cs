using System.Text.Json.Serialization;

namespace Config.Models
{
    public class ReprocessSettings
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = string.Empty;

        [JsonPropertyName("reference_log")]
        public string ReferenceLog { get; set; } = string.Empty;
    }
}
using System.Text.Json.Serialization;
using Commons.Interfaces;

namespace Config.Models
{
    public class ReprocessSettings : IReprocessSettings
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "ALL";

        [JsonPropertyName("reference_log")]
        public string ReferenceLog { get; set; } = string.Empty;
    }
}
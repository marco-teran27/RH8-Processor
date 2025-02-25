using System.Collections.Generic;
using System.Text.Json.Serialization;
using Commons.Interfaces;

namespace Config.Models
{
    public class PIDSettings : IPIDSettings
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = string.Empty;

        [JsonPropertyName("pids")]
        public List<string> Pids { get; set; } = new();
    }
}
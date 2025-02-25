using System.Collections.Generic;
using System.Text.Json.Serialization;
using Commons.Interfaces;

namespace Config.Models
{
    public class RhinoFileNameSettings : IRhinoFileNameSettings
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = string.Empty;

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; } = new();
    }
}
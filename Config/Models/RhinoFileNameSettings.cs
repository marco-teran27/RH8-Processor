using System.Text.Json.Serialization;

namespace Config.Models
{
    public class RhinoFileNameSettings
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("keywords")]
        public string[] Keywords { get; set; }

        [JsonPropertyName("rhino_file_name_pattern")]
        public string RhinoFileNamePattern { get; set; }
    }
}
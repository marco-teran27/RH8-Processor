using System.Text.Json.Serialization;
using Commons;

namespace Config.Models
{
    public class ScriptSettings
    {
        [JsonPropertyName("script_name")]
        public string ScriptName { get; set; } = string.Empty;

        [JsonPropertyName("script_type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ScriptType ScriptType { get; set; }
    }
}
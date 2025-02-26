using System.Text.Json.Serialization;
using Commons;
using Commons.Utils;
using Commons.Interfaces;

namespace Config.Models
{
    public class ScriptSettings : IScriptSettings
    {
        [JsonPropertyName("script_name")]
        public string ScriptName { get; set; } = string.Empty;

        [JsonPropertyName("script_type")]
        [JsonConverter(typeof(JsonStringEnumConverter))] // Added
        public ScriptType ScriptType { get; set; } = ScriptType.Python;
    }
}
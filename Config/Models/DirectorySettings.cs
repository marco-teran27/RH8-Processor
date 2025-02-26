using System.Text.Json.Serialization;
using Commons.Interfaces;

namespace Config.Models
{
    public class DirectorySettings : IDirectorySettings
    {
        [JsonPropertyName("file_dir")]
        public string FileDir { get; set; } = string.Empty;

        [JsonPropertyName("output_dir")]
        public string OutputDir { get; set; } = string.Empty;

        [JsonPropertyName("script_dir")]
        public string ScriptDir { get; set; } = string.Empty;
    }
}
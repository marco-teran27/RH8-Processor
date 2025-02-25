using System.Text.Json.Serialization;

namespace Config.Models
{
    public class DirectorySettings
    {
        [JsonPropertyName("file_dir")]
        public string FileDir { get; set; }

        [JsonPropertyName("output_dir")]
        public string OutputDir { get; set; }

        [JsonPropertyName("script_dir")]
        public string ScriptDir { get; set; }
    }
}
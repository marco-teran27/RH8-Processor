using System.Text.Json.Serialization;

namespace Config.Models
{
    public class ProjectName
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        // Not serialized, set programmatically by ConfigParser
        [JsonIgnore]
        public string ActualConfigFileName { get; set; }
    }
}
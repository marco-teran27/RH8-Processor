using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Config.Models
{
    public class ProjectName
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public string ActualConfigFileName { get; set; } = string.Empty;
    }

    public class ProjectNameConverter : JsonConverter<ProjectName>
    {
        public override ProjectName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new ProjectName { Name = reader.GetString() };
            }
            throw new JsonException("Expected a string for projectName.");
        }

        public override void Write(Utf8JsonWriter writer, ProjectName value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}
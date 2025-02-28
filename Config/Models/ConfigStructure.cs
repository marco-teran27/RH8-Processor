using System.Text.Json.Serialization;
using Commons.Interfaces;

namespace Config.Models
{
    public class ConfigStructure : IConfigStructure // Updated: Implement interface
    {
        [JsonPropertyName("projectName")]
        [JsonConverter(typeof(ProjectNameConverter))]
        public ProjectName ProjectName { get; set; } = new ProjectName();

        [JsonPropertyName("directories")]
        public DirectorySettings Directories { get; set; } = new DirectorySettings();

        [JsonPropertyName("pid_settings")]
        public PIDSettings PidSettings { get; set; } = new PIDSettings();

        [JsonPropertyName("rhino_file_name_settings")]
        public RhinoFileNameSettings RhinoFileNameSettings { get; set; } = new RhinoFileNameSettings();

        [JsonPropertyName("script_settings")]
        public ScriptSettings ScriptSettings { get; set; } = new ScriptSettings();

        [JsonPropertyName("reprocess_settings")]
        public ReprocessSettings ReprocessSettings { get; set; } = new ReprocessSettings();

        [JsonPropertyName("timeout_settings")]
        public TimeOutSettings TimeoutSettings { get; set; } = new TimeOutSettings();

        /// Updated: Explicit interface implementations for IConfigStructure
        string IConfigStructure.ProjectName => ProjectName.Value; // Assuming ProjectName has Value property
        IDirectorySettings IConfigStructure.Directories => Directories;
        IPIDSettings IConfigStructure.PIDSettings => PidSettings;
        IRhinoFileNameSettings IConfigStructure.RhinoFileNameSettings => RhinoFileNameSettings;
        IScriptSettings IConfigStructure.ScriptSettings => ScriptSettings;
        IReprocessSettings IConfigStructure.ReprocessSettings => ReprocessSettings;
        ITimeOutSettings IConfigStructure.TimeoutSettings => TimeoutSettings;
    }
}
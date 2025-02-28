using System.Text.Json.Serialization;
using Commons.Interfaces;

namespace Config.Models
{
    public class ConfigStructure : IConfigStructure
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

        /// Updated: Fix CS1061—Use ProjectName.Name instead of Value
        string IConfigStructure.ProjectName => ProjectName.Name;
        IDirectorySettings IConfigStructure.Directories => Directories;
        IPIDSettings IConfigStructure.PIDSettings => PidSettings;
        IRhinoFileNameSettings IConfigStructure.RhinoFileNameSettings => RhinoFileNameSettings;
        IScriptSettings IConfigStructure.ScriptSettings => ScriptSettings;
        IReprocessSettings IConfigStructure.ReprocessSettings => ReprocessSettings;
        ITimeOutSettings IConfigStructure.TimeoutSettings => TimeoutSettings;
    }
}
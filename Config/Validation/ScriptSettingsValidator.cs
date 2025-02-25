using System;
using System.IO;
using Config.Models;
using Config.Interfaces;
using Commons;

namespace Config.Validation
{
    public class ScriptSettingsValidator : IValidator
    {
        public (bool isValid, string errorMessage) ValidateConfig(
            ProjectName projectName,
            DirectorySettings directories,
            PIDSettings pidSettings,
            RhinoFileNameSettings rhinoFileNameSettings,
            ScriptSettings scriptSettings,
            ReprocessSettings reprocessSettings,
            TimeOutSettings timeoutSettings)
        {
            if (scriptSettings == null)
                return (false, "Script settings cannot be null.");

            if (string.IsNullOrWhiteSpace(scriptSettings.ScriptName))
                return (false, "script_settings.script_name cannot be empty.");

            var scriptPath = Path.Combine(directories.ScriptDir, scriptSettings.ScriptName);
            if (!File.Exists(scriptPath))
                return (false, $"Script file '{scriptPath}' does not exist.");

            if (scriptSettings.ScriptType == ScriptType.Unknown)
                return (false, "script_settings.script_type must be 'Python' or 'Grasshopper'.");

            return (true, string.Empty);
        }
    }
}
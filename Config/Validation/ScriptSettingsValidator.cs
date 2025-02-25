using System;
using System.IO;
using Config.Models;
using Commons;
using Config.Interfaces;

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

            bool allValid = true;
            string messages = "";

            if (string.IsNullOrWhiteSpace(scriptSettings.ScriptName))
                messages += "script_settings.script_name: missing; ";
            else
                messages += "script_settings.script_name: found; ";

            string? extension = scriptSettings.ScriptType switch
            {
                ScriptType.Python => ".py",
                ScriptType.Grasshopper => ".gh",
                ScriptType.GrasshopperXml => ".ghx",
                _ => null
            };

            if (extension == null)
                messages += "script_settings.script_type: needs to be 'Python', 'Grasshopper', or 'GrasshopperXml'; ";
            else
                messages += "script_settings.script_type: found; ";

            if (extension != null)
            {
                var scriptPath = Path.Combine(directories.ScriptDir, $"{scriptSettings.ScriptName}{extension}");
                if (!File.Exists(scriptPath))
                    messages += $"script_settings script file '{scriptPath}': missing; ";
                else
                    messages += $"script_settings script file '{scriptPath}': found; ";
            }

            allValid = !messages.Contains("missing") && !messages.Contains("needs to be");
            return (allValid, messages.TrimEnd(';'));
        }
    }
}
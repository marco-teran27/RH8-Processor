using System;
using System.Collections.Generic;
using System.IO;
using Config.Models;
using Config.Interfaces;
using Commons.Utils;
using Commons.Params;
using Commons.Interfaces;

namespace Config.Validation
{
    public class ScriptSettingsValidator : IValidator
    {
        public (bool isValid, IReadOnlyList<string> messages) ValidateConfig(
            ProjectName projectName,
            IDirectorySettings directories,
            IPIDSettings pidSettings,
            IRhinoFileNameSettings rhinoFileNameSettings,
            IScriptSettings scriptSettings,
            IReprocessSettings reprocessSettings,
            ITimeOutSettings timeoutSettings)
        {
            if (scriptSettings == null)
                return (false, new List<string> { "Script settings cannot be null." });

            bool allValid = true;
            var messages = new List<string>();

            if (string.IsNullOrWhiteSpace(scriptSettings.ScriptName))
                messages.Add("script_name: missing");
            else
                messages.Add("script_name: found");

            string? extension = scriptSettings.ScriptType switch
            {
                ScriptType.Python => ".py",
                ScriptType.Grasshopper => ".gh",
                ScriptType.GrasshopperXml => ".ghx",
                _ => null
            };

            if (extension == null)
                messages.Add("script_type: needs to be 'Python', 'Grasshopper', or 'GrasshopperXml'");
            else
                messages.Add("script_type: found");

            if (extension != null)
            {
                var scriptPath = Path.Combine(directories.ScriptDir, $"{scriptSettings.ScriptName}{extension}");
                if (!File.Exists(scriptPath))
                    messages.Add($"script file '{scriptPath}': missing");
                else
                    messages.Add($"script file '{scriptPath}': found");
            }

            allValid = !messages.Any(m => m.Contains("missing") || m.Contains("needs to be"));
            ScriptPath.Instance.SetScriptPath(scriptSettings, directories);
            return (allValid, messages);
        }
    }
}
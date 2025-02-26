using System;
using System.Collections.Generic;
using Config.Models;
using Config.Interfaces;
using Commons.Interfaces;

namespace Config.Validation
{
    public class RhinoFileNameValidator : IValidator
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
            if (rhinoFileNameSettings == null)
                return (false, new List<string> { "Rhino file name settings cannot be null." });

            bool allValid = true;
            var messages = new List<string>();

            if (string.IsNullOrWhiteSpace(rhinoFileNameSettings.Mode))
                messages.Add("rhino_file_name_settings.mode: missing");
            else if (!string.Equals(rhinoFileNameSettings.Mode, "list", StringComparison.OrdinalIgnoreCase) &&
                     !string.Equals(rhinoFileNameSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
                messages.Add($"rhino_file_name_settings.mode '{rhinoFileNameSettings.Mode}': needs to be 'list' or 'all'");
            else
                messages.Add("rhino_file_name_settings.mode: found");

            if (string.Equals(rhinoFileNameSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
                messages.Add("rhino_file_name_settings.keywords: Bypassed by ALL");
            else if (rhinoFileNameSettings.Keywords == null || rhinoFileNameSettings.Keywords.Count == 0)
                messages.Add("rhino_file_name_settings.keywords: missing");
            else
                messages.Add("rhino_file_name_settings.keywords: found");

            allValid = !messages.Any(m => m.Contains("missing") || m.Contains("needs to be"));
            return (allValid, messages);
        }
    }
}
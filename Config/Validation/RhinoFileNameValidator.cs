using System;
using Config.Models;
using Config.Interfaces;

namespace Config.Validation
{
    public class RhinoFileNameValidator : IValidator
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
            if (rhinoFileNameSettings == null)
                return (false, "Rhino file name settings cannot be null.");

            bool allValid = true;
            string messages = "";

            if (string.IsNullOrWhiteSpace(rhinoFileNameSettings.Mode))
                messages += "rhino_file_name_settings.mode: missing; ";
            else if (!string.Equals(rhinoFileNameSettings.Mode, "list", StringComparison.OrdinalIgnoreCase) &&
                     !string.Equals(rhinoFileNameSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
                messages += $"rhino_file_name_settings.mode '{rhinoFileNameSettings.Mode}': needs to be 'list' or 'all'; ";
            else
                messages += "rhino_file_name_settings.mode: found; ";

            if (string.Equals(rhinoFileNameSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
                messages += "rhino_file_name_settings.keywords: Bypassed by ALL; ";
            else if (rhinoFileNameSettings.Keywords == null || rhinoFileNameSettings.Keywords.Count == 0)
                messages += "rhino_file_name_settings.keywords: missing; ";
            else
                messages += "rhino_file_name_settings.keywords: found; ";

            allValid = !messages.Contains("missing") && !messages.Contains("needs to be");
            return (allValid, messages.TrimEnd(';'));
        }
    }
}
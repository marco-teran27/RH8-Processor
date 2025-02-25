using System;
using Commons;
using Config.Models;
using Config.Interfaces;

/*
File: Config\Validation\RhinoFileNameValidator.cs
Summary: Implements IValidator to validate Rhino file name settings.
         Ensures mode is valid and settings match expected patterns (list or regex).
*/

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

            if (string.IsNullOrWhiteSpace(rhinoFileNameSettings.Mode))
                return (false, "rhino_file_name_settings.mode cannot be empty.");

            var validModes = new[] { "list", "regex" };
            if (!Array.Exists(validModes, m => string.Equals(m, rhinoFileNameSettings.Mode, StringComparison.OrdinalIgnoreCase)))
                return (false, $"rhino_file_name_settings.mode '{rhinoFileNameSettings.Mode}' is not supported. Use 'list' or 'regex'.");

            if (string.Equals(rhinoFileNameSettings.Mode, "list", StringComparison.OrdinalIgnoreCase))
            {
                if (rhinoFileNameSettings.Keywords == null || rhinoFileNameSettings.Keywords.Length == 0)
                    return (false, "rhino_file_name_settings.keywords cannot be empty when mode is 'list'.");
            }
            else if (string.Equals(rhinoFileNameSettings.Mode, "regex", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(rhinoFileNameSettings.RhinoFileNamePattern))
                    return (false, "rhino_file_name_settings.rhino_file_name_pattern cannot be empty when mode is 'regex'.");

                if (!RhinoNameRegex.IsValidFileName(rhinoFileNameSettings.RhinoFileNamePattern))
                    return (false, $"rhino_file_name_settings.rhino_file_name_pattern '{rhinoFileNameSettings.RhinoFileNamePattern}' is not a valid regex pattern.");
            }

            return (true, string.Empty);
        }
    }
}
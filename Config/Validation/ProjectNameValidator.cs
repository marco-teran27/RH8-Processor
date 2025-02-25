using System;
using Config.Models;
using Commons;
using Config.Interfaces;

namespace Config.Validation
{
    public class ProjectNameValidator : IValidator
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
            return Validate(projectName, projectName.ActualConfigFileName);
        }

        public static (bool isValid, string errorMessage) Validate(ProjectName projectName, string actualConfigFileName)
        {
            if (string.IsNullOrWhiteSpace(projectName.Name))
                return (false, "projectName cannot be empty in the JSON config.");

            if (string.IsNullOrWhiteSpace(actualConfigFileName))
                return (false, "Actual config file name is missing (could not validate).");

            if (!ConfigNameRegex.IsValidConfigFileName(actualConfigFileName, out string extractedProjectName))
                return (false, $"Config file '{actualConfigFileName}' does not follow 'config-{{ProjectName}}.json' naming.");

            if (!string.Equals(projectName.Name, extractedProjectName, StringComparison.OrdinalIgnoreCase))
                return (false, $"Mismatch: JSON projectName = '{projectName.Name}', file name substring = '{extractedProjectName}'.");

            return (true, string.Empty);
        }
    }
}
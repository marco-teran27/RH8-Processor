using System;
using System.Collections.Generic;
using Config.Models;
using Config.Interfaces;
using Commons.Utils;

namespace Config.Validation
{
    public class ProjectNameValidator : IValidator
    {
        public (bool isValid, IReadOnlyList<string> messages) ValidateConfig(
            ProjectName projectName,
            DirectorySettings directories,
            PIDSettings pidSettings,
            RhinoFileNameSettings rhinoFileNameSettings,
            ScriptSettings scriptSettings,
            ReprocessSettings reprocessSettings,
            TimeOutSettings timeoutSettings)
        {
            var messages = new List<string>();

            if (string.IsNullOrWhiteSpace(projectName.Name))
                messages.Add("projectName: missing");
            else if (string.IsNullOrWhiteSpace(projectName.ActualConfigFileName))
                messages.Add("Actual config file name: missing");
            else if (!ConfigNameRegex.IsValidConfigFileName(projectName.ActualConfigFileName, out string extractedProjectName))
                messages.Add($"Config file '{projectName.ActualConfigFileName}': does not follow 'config-{{ProjectName}}.json' naming");
            else if (!string.Equals(projectName.Name, extractedProjectName, StringComparison.OrdinalIgnoreCase))
                messages.Add($"Mismatch: JSON projectName = '{projectName.Name}', file name substring = '{extractedProjectName}'");
            else
                messages.Add("Passed");

            bool allValid = !messages.Any(m => m.Contains("missing") || m.Contains("Mismatch") || m.Contains("does not follow"));
            return (allValid, messages);
        }
    }
}
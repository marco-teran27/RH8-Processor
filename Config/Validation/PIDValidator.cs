using System;
using System.Collections.Generic;
using Config.Models;
using Config.Interfaces;

namespace Config.Validation
{
    public class PIDValidator : IValidator
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
            if (pidSettings == null)
                return (false, new List<string> { "PID settings cannot be null." });

            bool allValid = true;
            var messages = new List<string>();

            if (string.IsNullOrWhiteSpace(pidSettings.Mode))
                messages.Add("pid_settings.mode: missing");
            else if (!string.Equals(pidSettings.Mode, "list", StringComparison.OrdinalIgnoreCase) &&
                     !string.Equals(pidSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
                messages.Add($"pid_settings.mode '{pidSettings.Mode}': needs to be 'list' or 'all'");
            else
                messages.Add("pid_settings.mode: found");

            if (string.Equals(pidSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
                messages.Add("pid_settings.pids: Bypassed by ALL");
            else if (pidSettings.Pids == null || pidSettings.Pids.Count == 0)
                messages.Add("pid_settings.pids: missing");
            else
                messages.Add("pid_settings.pids: found");

            allValid = !messages.Any(m => m.Contains("missing") || m.Contains("needs to be"));
            return (allValid, messages);
        }
    }
}
using System;
using Config.Models;
using Config.Interfaces;

namespace Config.Validation
{
    public class PIDValidator : IValidator
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
            if (pidSettings == null)
                return (false, "PID settings cannot be null.");

            bool allValid = true;
            string messages = "";

            if (string.IsNullOrWhiteSpace(pidSettings.Mode))
                messages += "pid_settings.mode: missing; ";
            else if (!string.Equals(pidSettings.Mode, "list", StringComparison.OrdinalIgnoreCase) &&
                     !string.Equals(pidSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
                messages += $"pid_settings.mode '{pidSettings.Mode}': needs to be 'list' or 'all'; ";
            else
                messages += "pid_settings.mode: found; ";

            if (string.Equals(pidSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
                messages += "pid_settings.pids: Bypassed by ALL; ";
            else if (pidSettings.Pids == null || pidSettings.Pids.Count == 0) // Fixed: Count for List<string>
                messages += "pid_settings.pids: missing; ";
            else
                messages += "pid_settings.pids: found; ";

            allValid = !messages.Contains("missing") && !messages.Contains("needs to be");
            return (allValid, messages.TrimEnd(';'));
        }
    }
}
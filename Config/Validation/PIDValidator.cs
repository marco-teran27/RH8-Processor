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

            if (string.IsNullOrWhiteSpace(pidSettings.Mode))
                return (false, "pid_settings.mode cannot be empty.");

            if (!string.Equals(pidSettings.Mode, "list", StringComparison.OrdinalIgnoreCase))
                return (false, $"pid_settings.mode '{pidSettings.Mode}' is not supported. Only 'list' is valid.");

            if (pidSettings.Pids == null || pidSettings.Pids.Length == 0)
                return (false, "pid_settings.pids cannot be empty when mode is 'list'.");

            foreach (var pid in pidSettings.Pids)
            {
                if (string.IsNullOrWhiteSpace(pid))
                    return (false, "pid_settings.pids contains an empty or null PID.");
            }

            return (true, string.Empty);
        }
    }
}
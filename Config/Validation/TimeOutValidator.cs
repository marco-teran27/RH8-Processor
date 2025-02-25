using System;
using Config.Models;
using Config.Interfaces;

namespace Config.Validation
{
    public class TimeOutValidator : IValidator
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
            if (timeoutSettings == null)
                return (false, "Timeout settings cannot be null.");

            bool allValid = true;
            string messages = "";

            if (timeoutSettings.Minutes <= 0)
                messages += "timeout_settings.minutes: missing or invalid (must be > 0); ";
            else
                messages += "timeout_settings.minutes: found; ";

            allValid = !messages.Contains("missing");
            return (allValid, messages.TrimEnd(';'));
        }
    }
}
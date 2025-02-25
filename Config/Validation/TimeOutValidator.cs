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

            if (timeoutSettings.Minutes <= 0)
                return (false, "timeout_settings.minutes must be greater than 0.");

            return (true, string.Empty);
        }
    }
}
using System;
using System.Collections.Generic;
using Config.Models;
using Config.Interfaces;
using Commons.Interfaces;

namespace Config.Validation
{
    public class TimeOutValidator : IValidator
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
            if (timeoutSettings == null)
                return (false, new List<string> { "Timeout settings cannot be null." });

            var messages = new List<string>();

            if (timeoutSettings.Minutes <= 0)
                messages.Add("timeout_settings.minutes: missing or invalid (must be > 0)");
            else
                messages.Add("timeout_settings.minutes: found");

            bool allValid = !messages.Any(m => m.Contains("missing"));
            return (allValid, messages);
        }
    }
}
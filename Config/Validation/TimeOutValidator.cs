using System;
using System.Collections.Generic;
using Config.Interfaces;
using Commons.Params;
using Commons.Interfaces;
using Config.Models;

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
            int adjustedMinutes = timeoutSettings.Minutes;

            if (adjustedMinutes < 1)
            {
                messages.Add($"minutes: adjusted from {adjustedMinutes} to 1 (must be > 0)");
                adjustedMinutes = 1;
            }
            else
            {
                messages.Add("minutes: found");
            }

            TimeOutMin.Instance.SetMinutes(new TimeOutSettings { Minutes = adjustedMinutes });
            return (true, messages); // Always valid post-adjustment
        }
    }
}
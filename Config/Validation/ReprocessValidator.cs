using System;
using System.Collections.Generic;
using System.IO;
using Config.Models;
using Config.Interfaces;
using Commons.Params;
using Commons.Interfaces;

namespace Config.Validation
{
    public class ReprocessValidator : IValidator
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
            if (reprocessSettings == null)
                return (false, new List<string> { "Reprocess settings cannot be null." });

            bool allValid = true;
            var messages = new List<string>();

            if (string.IsNullOrWhiteSpace(reprocessSettings.Mode))
                messages.Add("mode: missing");
            else
            {
                var validModes = new[] { "ALL", "PASS", "FAIL", "RESUME" };
                if (!Array.Exists(validModes, m => string.Equals(m, reprocessSettings.Mode, StringComparison.OrdinalIgnoreCase)))
                    messages.Add($"mode '{reprocessSettings.Mode}': needs to be 'ALL', 'PASS', 'FAIL', or 'RESUME'");
                else
                    messages.Add("mode: found");
            }

            if (string.Equals(reprocessSettings.Mode, "ALL", StringComparison.OrdinalIgnoreCase))
            {
                messages.Add("reference_log: Bypassed by ALL");
            }
            else if (string.IsNullOrWhiteSpace(reprocessSettings.ReferenceLog))
            {
                messages.Add("reference_log: Path to reference log file is missing");
                allValid = false;
            }
            else if (!File.Exists(reprocessSettings.ReferenceLog) || !reprocessSettings.ReferenceLog.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                messages.Add($"reference_log '{reprocessSettings.ReferenceLog}': missing or not a .json file");
                allValid = false;
            }
            else
            {
                messages.Add($"reference_log '{reprocessSettings.ReferenceLog}': found");
            }

            Reprocess.Instance.SetReprocess(reprocessSettings);
            return (allValid, messages);
        }
    }
}
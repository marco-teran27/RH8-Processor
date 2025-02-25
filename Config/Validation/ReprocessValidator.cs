using System;
using System.Collections.Generic;
using System.IO;
using Config.Models;
using Config.Interfaces;

namespace Config.Validation
{
    public class ReprocessValidator : IValidator
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
            if (reprocessSettings == null)
                return (false, new List<string> { "Reprocess settings cannot be null." });

            bool allValid = true;
            var messages = new List<string>();

            if (string.IsNullOrWhiteSpace(reprocessSettings.Mode))
                messages.Add("reprocess_settings.mode: missing");
            else
            {
                var validModes = new[] { "ALL", "PASS", "FAIL", "RESUME" };
                if (!Array.Exists(validModes, m => string.Equals(m, reprocessSettings.Mode, StringComparison.OrdinalIgnoreCase)))
                    messages.Add($"reprocess_settings.mode '{reprocessSettings.Mode}': needs to be 'ALL', 'PASS', 'FAIL', or 'RESUME'");
                else
                    messages.Add("reprocess_settings.mode: found");
            }

            if (string.Equals(reprocessSettings.Mode, "ALL", StringComparison.OrdinalIgnoreCase))
                messages.Add("reprocess_settings.reference_log: Bypassed by ALL");
            else if (string.IsNullOrWhiteSpace(reprocessSettings.ReferenceLog))
                messages.Add("reprocess_settings.reference_log: Path to reference log file is missing");
            else if (!File.Exists(reprocessSettings.ReferenceLog))
                messages.Add($"reprocess_settings.reference_log '{reprocessSettings.ReferenceLog}': missing");
            else
                messages.Add($"reprocess_settings.reference_log '{reprocessSettings.ReferenceLog}': found");

            allValid = !messages.Any(m => m.Contains("missing") || m.Contains("needs to be"));
            return (allValid, messages);
        }
    }
}
using System;
using Config.Models;
using Config.Interfaces;

namespace Config.Validation
{
    public class ReprocessValidator : IValidator
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
            if (reprocessSettings == null)
                return (false, "Reprocess settings cannot be null.");

            if (string.IsNullOrWhiteSpace(reprocessSettings.Mode))
                return (false, "reprocess_settings.mode cannot be empty.");

            var validModes = new[] { "all", "selective" };
            if (!Array.Exists(validModes, m => string.Equals(m, reprocessSettings.Mode, StringComparison.OrdinalIgnoreCase)))
                return (false, $"reprocess_settings.mode '{reprocessSettings.Mode}' is not supported. Use 'all' or 'selective'.");

            if (string.Equals(reprocessSettings.Mode, "selective", StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrWhiteSpace(reprocessSettings.ReferenceLog))
                return (false, "reprocess_settings.reference_log cannot be empty when mode is 'selective'.");

            return (true, string.Empty);
        }
    }
}
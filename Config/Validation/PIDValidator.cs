using System;
using System.Collections.Generic;
using Config.Models;
using Config.Interfaces;
using Commons.Utils;
using Commons.Params;
using System.Collections;
using System.Net.NetworkInformation;

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
            var pidStatuses = new List<PIDStatus>();

            if (string.IsNullOrWhiteSpace(pidSettings.Mode))
                messages.Add("mode: missing");
            else if (!string.Equals(pidSettings.Mode, "list", StringComparison.OrdinalIgnoreCase) &&
                     !string.Equals(pidSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
                messages.Add($"mode '{pidSettings.Mode}': needs to be 'list' or 'all'");
            else
                messages.Add("mode: found");

            if (string.Equals(pidSettings.Mode, "all", StringComparison.OrdinalIgnoreCase))
            {
                messages.Add("pids: Bypassed by ALL");
            }
            else if (pidSettings.Pids == null || pidSettings.Pids.Count == 0)
            {
                messages.Add("pids: missing");
                allValid = false;
            }
            else
            {
                bool allPidsValid = true;
                foreach (var pid in pidSettings.Pids)
                {
                    if (string.IsNullOrWhiteSpace(pid))
                    {
                        pidStatuses.Add(new PIDStatus(pid, false));
                        allPidsValid = false;
                    }
                    else if (!PatientIDRegex.Pattern.IsMatch(pid))
                    {
                        pidStatuses.Add(new PIDStatus(pid, false));
                        allPidsValid = false;
                    }
                    else
                    {
                        pidStatuses.Add(new PIDStatus(pid, true));
                    }
                }
                messages.Add($"pids: {(allPidsValid ? "found with valid formatting" : "found with invalid formatting")}");
                allValid &= allPidsValid;
            }

            PIDListFormat.Instance.SetPids(pidStatuses);
            PIDList.Instance.CompileIds(pidSettings, rhinoFileNameSettings);

            return (allValid, messages);
        }
    }
}
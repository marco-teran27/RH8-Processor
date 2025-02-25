using System.Collections.Generic;
using System.Linq;
using Interfaces;

namespace Commons.Logging
{
    public static class ConfigValLog
    {
        public static void LogValidationResults(ConfigValidationResults results, IRhinoCommOut rhinoCommOut)
        {
            foreach (var result in results.ValidatorResults)
            {
                string validatorName = result.ValidatorName switch
                {
                    "ProjectName" => "PROJECT NAME",
                    "Directory" => "DIRECTORIES",
                    "PID" => "PID SETTINGS",
                    "Reprocess" => "REPROCESS SETTINGS",
                    "RhinoFileName" => "RHINO FILE NAME SETTINGS",
                    "ScriptSettings" => "SCRIPT SETTINGS",
                    "TimeOut" => "TIMEOUT SETTINGS",
                    _ => result.ValidatorName.ToUpper()
                };

                if (result.IsValid)
                    rhinoCommOut.ShowMessage($"{validatorName}:");
                else
                    rhinoCommOut.ShowError($"{validatorName}:");

                foreach (var message in result.Messages)
                {
                    // Remove model-specific prefixes
                    string cleanMessage = message switch
                    {
                        var m when m.StartsWith("pid_settings.") => m.Replace("pid_settings.", ""),
                        var m when m.StartsWith("reprocess_settings.") => m.Replace("reprocess_settings.", ""),
                        var m when m.StartsWith("rhino_file_name_settings.") => m.Replace("rhino_file_name_settings.", ""),
                        var m when m.StartsWith("script_settings ") => m.Replace("script_settings ", ""),
                        var m when m.StartsWith("script_settings.") => m.Replace("script_settings.", ""),
                        var m when m.StartsWith("timeout_settings.") => m.Replace("timeout_settings.", ""),
                        _ => message
                    };

                    if (result.IsValid)
                        rhinoCommOut.ShowMessage(cleanMessage);
                    else
                        rhinoCommOut.ShowError(cleanMessage);
                }
            }

            if (results.IsValid)
                rhinoCommOut.ShowMessage("All validations passed.");
            else
                rhinoCommOut.ShowError("Validation failed. Please address the issues above.");
        }
    }
}
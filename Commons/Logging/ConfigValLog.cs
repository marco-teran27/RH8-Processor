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

                // Header with "Error:" if invalid
                string header = result.IsValid ? $"{validatorName}:" : $"Error: {validatorName}:";
                rhinoCommOut.ShowMessage(header);

                foreach (var message in result.Messages)
                {
                    // Strip model-specific prefixes
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

                    // Only prefix "Error:" to failing sub-items via ShowError
                    bool isError = cleanMessage.Contains("missing") || cleanMessage.Contains("invalid") || cleanMessage.Contains("needs to be");
                    if (result.IsValid || !isError)
                        rhinoCommOut.ShowMessage(cleanMessage);
                    else
                        rhinoCommOut.ShowError(cleanMessage); // ShowError adds "Error:" once
                }
            }

            if (results.IsValid)
                rhinoCommOut.ShowMessage("All validations passed.");
            else
                rhinoCommOut.ShowError("Validation failed. Please address the issues above.");
        }
    }
}
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
                    // Only prefix "Error:" to failing sub-items
                    bool isError = message.Contains("missing") || message.Contains("invalid") || message.Contains("needs to be");
                    string formattedMessage = isError ? $"Error: {message}" : message;

                    if (result.IsValid || !isError)
                        rhinoCommOut.ShowMessage(formattedMessage);
                    else
                        rhinoCommOut.ShowError(formattedMessage);
                }
            }

            if (results.IsValid)
                rhinoCommOut.ShowMessage("All validations passed.");
            else
                rhinoCommOut.ShowError("Validation failed. Please address the issues above.");
        }
    }
}
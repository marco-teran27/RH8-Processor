using System.Collections.Generic;
using System.IO;
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

                string header = result.IsValid ? $"{validatorName}:" : $"Error: {validatorName}:";
                rhinoCommOut.ShowMessage(header);

                foreach (var message in result.Messages)
                {
                    bool isError = message.Contains("missing") || message.Contains("invalid") || message.Contains("needs to be");
                    string cleanMessage = message;

                    if (validatorName == "DIRECTORIES")
                    {
                        /// Updated: Use ScriptDir instead of FullPath for script_dir
                        if (message.Contains("file_dir"))
                            cleanMessage = $"file_dir: {Path.GetFileName(Commons.Params.BatchDir.Instance.FileDir)}: {message.Split(':').Last().Trim()}";
                        else if (message.Contains("output_dir"))
                            cleanMessage = $"output_dir: {Path.GetFileName(Commons.Params.BatchDir.Instance.OutputDir)}: {message.Split(':').Last().Trim()}";
                        else if (message.Contains("script_dir"))
                            cleanMessage = $"script_dir: {Path.GetFileName(Commons.Params.ScriptPath.Instance.ScriptDir)}: {message.Split(':').Last().Trim()}";
                    }
                    else if (validatorName == "SCRIPT SETTINGS" && message.Contains("script file"))
                    {
                        continue;
                    }
                    else if (validatorName == "RHINO FILE NAME SETTINGS")
                    {
                        cleanMessage = message.Replace("rhino_file_name_settings.", "");
                    }

                    string formattedMessage = isError ? $"Error: {cleanMessage}" : cleanMessage;
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
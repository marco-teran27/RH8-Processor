using System.Collections.Generic;
using System.IO;
using System.Linq;
using Interfaces;
using Config; // For ConfigStructure

namespace Commons.LogComm
{
    public static class ConfigValComm
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

                rhinoCommOut.ShowMessage($"\n{validatorName}");

                foreach (var message in result.Messages)
                {
                    bool isError = message.Contains("missing") || message.Contains("invalid") || message.Contains("needs to be");
                    string cleanMessage = message;

                    if (validatorName == "DIRECTORIES")
                    {
                        if (message.Contains("file_dir"))
                            cleanMessage = isError ? "file_dir: ERROR NOT FOUND" : $"file_dir: {Path.GetFileName(Commons.Params.BatchDir.Instance.FileDir)}";
                        else if (message.Contains("output_dir"))
                            cleanMessage = isError ? "output_dir: ERROR NOT FOUND" : $"output_dir: {Path.GetFileName(Commons.Params.BatchDir.Instance.OutputDir)}";
                        else if (message.Contains("script_dir"))
                            cleanMessage = isError ? "script_dir: ERROR NOT FOUND" : $"script_dir: {Path.GetFileName(Commons.Params.ScriptPath.Instance.ScriptDir)}";
                    }
                    else if (validatorName == "SCRIPT SETTINGS")
                    {
                        if (message.Contains("script_name"))
                            cleanMessage = isError ? "script_name: ERROR NOT FOUND" : $"script_name: {results.ConfigStructure.ScriptSettings.ScriptName}";
                        else if (message.Contains("script_type"))
                            cleanMessage = isError ? "script_type: ERROR NOT FOUND" : $"script_type: {results.ConfigStructure.ScriptSettings.ScriptType}";
                        else if (message.Contains("script file"))
                            continue;
                    }
                    else if (validatorName == "RHINO FILE NAME SETTINGS")
                    {
                        /// Fixed: Use ConfigStructure for Mode and Keywords
                        if (message.Contains("mode"))
                            cleanMessage = isError ? "mode: ERROR NOT FOUND" : $"mode: {results.ConfigStructure.RhinoFileNameSettings.Mode}";
                        else if (message.Contains("keywords"))
                            cleanMessage = isError ? "keywords: ERROR NOT FOUND" : $"keywords: {(results.ConfigStructure.RhinoFileNameSettings.Mode == "ALL" ? "Bypassed by ALL" : string.Join(",", results.ConfigStructure.RhinoFileNameSettings.Keywords))}";
                    }
                    else if (validatorName == "TIMEOUT SETTINGS")
                    {
                        cleanMessage = isError ? "minutes: ERROR NOT FOUND" : $"minutes: {Commons.Params.TimeOutMin.Instance.Minutes}";
                    }
                    else if (validatorName == "PID SETTINGS" && message.Contains("pids"))
                    {
                        cleanMessage = isError ? "pids: ERROR NOT FOUND" : $"pids: {cleanMessage.Split(':').Last().Trim()}";
                    }
                    else if (validatorName == "PID SETTINGS")
                    {
                        /// Fixed: Use ConfigStructure for Mode
                        cleanMessage = isError ? "mode: ERROR NOT FOUND" : $"mode: {results.ConfigStructure.PIDSettings.Mode}";
                    }
                    else if (validatorName == "REPROCESS SETTINGS")
                    {
                        if (message.Contains("mode"))
                            cleanMessage = isError ? "mode: ERROR NOT FOUND" : $"mode: {Commons.Params.Reprocess.Instance.Mode}";
                        else if (message.Contains("reference_log"))
                            cleanMessage = isError ? "reference_log: ERROR NOT FOUND" : $"reference_log: {(string.IsNullOrEmpty(Commons.Params.Reprocess.Instance.ReferenceLog) ? "Bypassed by ALL" : Commons.Params.Reprocess.Instance.ReferenceLog)}";
                    }
                    else if (validatorName == "PROJECT NAME")
                    {
                        cleanMessage = isError ? $"ERROR {cleanMessage.Split(':').Last().Trim().ToUpper()}" : cleanMessage.Split(':').Last().Trim();
                    }

                    rhinoCommOut.ShowMessage(cleanMessage);
                }
            }

            if (results.IsValid)
                rhinoCommOut.ShowMessage("\nALL VALIDATIONS PASSED.");
            else
                rhinoCommOut.ShowError("VALIDATION FAILED. PLEASE ADDRESS THE ISSUES ABOVE.");
        }
    }
}
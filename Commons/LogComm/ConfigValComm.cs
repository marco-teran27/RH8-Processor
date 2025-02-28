using System.Collections.Generic;
using System.IO;
using System.Linq;
using Interfaces;
using Commons.Params;

namespace Commons.LogComm
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

                rhinoCommOut.ShowMessage($"\n{validatorName}");

                foreach (var message in result.Messages)
                {
                    bool isError = message.Contains("missing") || message.Contains("invalid") || message.Contains("needs to be");
                    string cleanMessage = message;

                    if (validatorName == "DIRECTORIES")
                    {
                        /// Updated: Fixed error format to "ERROR NOT FOUND"
                        if (message.Contains("file_dir"))
                            cleanMessage = isError ? "file_dir: ERROR NOT FOUND" : $"file_dir: {Path.GetFileName(BatchDir.Instance.FileDir)}";
                        else if (message.Contains("output_dir"))
                            cleanMessage = isError ? "output_dir: ERROR NOT FOUND" : $"output_dir: {Path.GetFileName(BatchDir.Instance.OutputDir)}";
                        else if (message.Contains("script_dir"))
                            cleanMessage = isError ? "script_dir: ERROR NOT FOUND" : $"script_dir: {Path.GetFileName(ScriptPath.Instance.ScriptDir)}";
                    }
                    else if (validatorName == "SCRIPT SETTINGS")
                    {
                        if (message.Contains("script_name"))
                            cleanMessage = isError ? "script_name: ERROR NOT FOUND" : $"script_name: {ScriptPath.Instance.FullPath.Split(Path.DirectorySeparatorChar).Last().Replace(".py", "").Replace(".gh", "").Replace(".ghx", "")}";
                        else if (message.Contains("script_type"))
                        {
                            string scriptType = ScriptPath.Instance.FullPath.EndsWith(".py") ? "Python" : ScriptPath.Instance.FullPath.EndsWith(".gh") ? "Grasshopper" : "GrasshopperXml";
                            cleanMessage = isError ? "script_type: ERROR NOT FOUND" : $"script_type: {scriptType}";
                        }
                        else if (message.Contains("script file"))
                            continue;
                    }
                    else if (validatorName == "RHINO FILE NAME SETTINGS")
                    {
                        /// Updated: Use Commons.Params values, fix keywords logic
                        if (message.Contains("mode"))
                            cleanMessage = isError ? "mode: ERROR NOT FOUND" : $"mode: {RhinoFileNameList.Instance.Mode}";
                        else if (message.Contains("keywords"))
                            cleanMessage = isError ? "keywords: ERROR NOT FOUND" : $"keywords: {(RhinoFileNameList.Instance.Mode == "ALL" ? "Bypassed by ALL" : string.Join(",", RhinoFileNameList.Instance.GetKeywords()))}";
                    }
                    else if (validatorName == "TIMEOUT SETTINGS")
                    {
                        /// Updated: Use Commons.Params.Minutes
                        cleanMessage = isError ? "minutes: ERROR NOT FOUND" : $"minutes: {TimeOutMin.Instance.Minutes}";
                    }
                    else if (validatorName == "PID SETTINGS" && message.Contains("pids"))
                    {
                        cleanMessage = isError ? "pids: ERROR NOT FOUND" : $"pids: {cleanMessage.Split(':').Last().Trim()}";
                    }
                    else if (validatorName == "PID SETTINGS")
                    {
                        cleanMessage = isError ? "mode: ERROR NOT FOUND" : $"mode: {PIDList.Instance.Mode}";
                    }
                    else if (validatorName == "REPROCESS SETTINGS")
                    {
                        /// Updated: Use Commons.Params values
                        if (message.Contains("mode"))
                            cleanMessage = isError ? "mode: ERROR NOT FOUND" : $"mode: {Reprocess.Instance.Mode}";
                        else if (message.Contains("reference_log"))
                            cleanMessage = isError ? "reference_log: ERROR NOT FOUND" : $"reference_log: {(string.IsNullOrEmpty(Reprocess.Instance.ReferenceLog) ? "Bypassed by ALL" : Reprocess.Instance.ReferenceLog)}";
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
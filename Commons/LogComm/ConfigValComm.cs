using System.Collections.Generic;
using Interfaces;

namespace Commons.LogComm
{
    public static class ConfigValComm
    {
        public static void LogValidationResults(IConfigValResults results, IRhinoCommOut rhinoCommOut)
        {
            foreach (var (name, isValid, messages) in results.Results)
            {
                string validatorName = name switch
                {
                    "ProjectName" => "PROJECT NAME",
                    "FileDir" => "FILE DIR",
                    "OutputDir" => "OUTPUT DIR",
                    "ScriptDir" => "SCRIPT DIR",
                    "ScriptName" => "SCRIPT NAME",
                    "ScriptType" => "SCRIPT TYPE",
                    "RhinoFileMode" => "RHINO FILE MODE",
                    "RhinoFileKeywords" => "RHINO FILE KEYWORDS",
                    "PidMode" => "PID MODE",
                    "Pids" => "PIDS",
                    "ReprocessMode" => "REPROCESS MODE",
                    "ReferenceLog" => "REFERENCE LOG",
                    "TimeoutMinutes" => "TIMEOUT MINUTES",
                    "ConfigFile" => "CONFIG FILE",
                    _ => name.ToUpper()
                };

                rhinoCommOut.ShowMessage($"\n{validatorName}");
                foreach (var message in messages)
                {
                    bool isError = message.Contains("missing") || message.Contains("invalid") || message.Contains("needs") || message.Contains("Mismatch");
                    rhinoCommOut.ShowMessage(isError ? $"ERROR: {message}" : message);
                }
            }

            rhinoCommOut.ShowMessage(results.IsValid ? "\nALL VALIDATIONS PASSED." : "\nVALIDATION FAILED.");
        }
    }
}
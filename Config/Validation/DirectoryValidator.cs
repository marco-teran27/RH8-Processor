using System;
using System.IO;
using Config.Models;
using Config.Interfaces;

namespace Config.Validation
{
    public class DirectoryValidator : IValidator
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
            if (directories == null)
                return (false, "Directory settings cannot be null.");

            bool allValid = true;
            string messages = "";

            if (string.IsNullOrWhiteSpace(directories.FileDir))
                messages += "file_dir: missing; ";
            else if (!Directory.Exists(directories.FileDir))
                messages += $"file_dir '{directories.FileDir}': missing; ";
            else
                messages += "file_dir: found; ";

            if (string.IsNullOrWhiteSpace(directories.OutputDir))
                messages += "output_dir: missing; ";
            else if (!Directory.Exists(directories.OutputDir))
                messages += $"output_dir '{directories.OutputDir}': missing; ";
            else
                messages += "output_dir: found; ";

            if (string.IsNullOrWhiteSpace(directories.ScriptDir))
                messages += "script_dir: missing; ";
            else if (!Directory.Exists(directories.ScriptDir))
                messages += $"script_dir '{directories.ScriptDir}': missing; ";
            else
                messages += "script_dir: found; ";

            allValid = !messages.Contains("missing");
            return (allValid, messages.TrimEnd(';'));
        }
    }
}
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

            if (string.IsNullOrWhiteSpace(directories.FileDir))
                return (false, "file_dir cannot be empty in directory settings.");
            if (!Directory.Exists(directories.FileDir))
                return (false, $"file_dir '{directories.FileDir}' does not exist.");

            if (string.IsNullOrWhiteSpace(directories.OutputDir))
                return (false, "output_dir cannot be empty in directory settings.");
            if (!Directory.Exists(directories.OutputDir))
                return (false, $"output_dir '{directories.OutputDir}' does not exist.");

            if (string.IsNullOrWhiteSpace(directories.ScriptDir))
                return (false, "script_dir cannot be empty in directory settings.");
            if (!Directory.Exists(directories.ScriptDir))
                return (false, $"script_dir '{directories.ScriptDir}' does not exist.");

            return (true, string.Empty);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Config.Models;
using Config.Interfaces;
using Commons.Params;
using Commons.Interfaces;

namespace Config.Validation
{
    public class DirectoryValidator : IValidator
    {
        public (bool isValid, IReadOnlyList<string> messages) ValidateConfig(
            ProjectName projectName,
            IDirectorySettings directories,
            IPIDSettings pidSettings,
            IRhinoFileNameSettings rhinoFileNameSettings,
            IScriptSettings scriptSettings,
            IReprocessSettings reprocessSettings,
            ITimeOutSettings timeoutSettings)
        {
            if (directories == null)
                return (false, new List<string> { "Directory settings cannot be null." });

            bool allValid = true;
            var messages = new List<string>();

            if (string.IsNullOrWhiteSpace(directories.FileDir))
                messages.Add("file_dir: missing");
            else if (!Directory.Exists(directories.FileDir))
                messages.Add($"file_dir '{directories.FileDir}': missing");
            else
                messages.Add("file_dir: found");

            if (string.IsNullOrWhiteSpace(directories.OutputDir))
                messages.Add("output_dir: missing");
            else if (!Directory.Exists(directories.OutputDir))
                messages.Add($"output_dir '{directories.OutputDir}': missing");
            else
                messages.Add("output_dir: found");

            if (string.IsNullOrWhiteSpace(directories.ScriptDir))
                messages.Add("script_dir: missing");
            else if (!Directory.Exists(directories.ScriptDir))
                messages.Add($"script_dir '{directories.ScriptDir}': missing");
            else
                messages.Add("script_dir: found");

            allValid = !messages.Any(m => m.Contains("missing"));
            BatchDir.Instance.SetDirectories(directories);
            return (allValid, messages);
        }
    }
}
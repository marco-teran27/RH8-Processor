using System;
using System.Collections.Generic;
using System.IO;
using Commons;

namespace Config.Models
{
    public class LoggingConfig
    {
        public int CommandLineBufferSize { get; set; } = 1000;
        public bool EnableDebugLogging { get; set; } = false;
        public string LogDirectory { get; set; } = string.Empty;
        public bool PreserveCompletionFiles { get; set; } = false;
        public int MaxLogFileSize { get; set; } = 10 * 1024 * 1024; // 10MB default

        public ConfigValidationResults Validate()
        {
            var errors = new List<string>();

            if (CommandLineBufferSize <= 0)
                errors.Add("Command line buffer size must be greater than 0");

            if (MaxLogFileSize <= 0)
                errors.Add("Max log file size must be greater than 0");

            if (!string.IsNullOrEmpty(LogDirectory) && !Directory.Exists(LogDirectory))
                errors.Add($"Log directory not found: {LogDirectory}");

            return new ConfigValidationResults(errors.Count == 0, errors);
        }
    }
}
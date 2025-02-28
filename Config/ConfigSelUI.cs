using System;
using System.IO;
using Commons.Interfaces;
using Config.Interfaces;
using Interfaces;

namespace Config
{
    public class ConfigSelUI : IConfigSelUI
    {
        private readonly IRhinoCommOut _rhinoCommOut;

        public ConfigSelUI(IRhinoCommOut rhinoCommOut)
        {
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
        }

        public IRhinoCommOut RhinoCommOut => _rhinoCommOut;

        public string SelectConfigFile()
        {
            try
            {
                _rhinoCommOut.ShowMessage("\nstarting batchprocessor");
                string configPath = Console.ReadLine(); // Placeholder—replace with actual UI logic
                if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
                {
                    _rhinoCommOut.ShowError("CONFIGURATION SELECTION CANCELED.");
                    return null;
                }
                _rhinoCommOut.ShowMessage($"\nCONFIG FILE SELECTED: {Path.GetFileName(configPath)}");
                return configPath;
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"CONFIG SELECTION FAILED: {ex.Message}");
                return null;
            }
        }
    }
}
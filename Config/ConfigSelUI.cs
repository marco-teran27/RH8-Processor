using System;
using System.IO;
using System.Threading;
using System.Windows.Forms; // For OpenFileDialog
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

        public string SelectConfigFile()
        {
            string configPath = null;
            Exception dialogException = null;

            // Run OpenFileDialog on STA thread
            Thread staThread = new Thread(() =>
            {
                try
                {
                    _rhinoCommOut.ShowMessage("\nstarting batchprocessor");
                    using OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = true,
                        Multiselect = false,
                        InitialDirectory = @"C:\", // TODO: Replace with Environment.SpecialFolder.Desktop or persisted state
                        Title = "Select Config File"
                    };

                    DialogResult result = openFileDialog.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
                    {
                        configPath = openFileDialog.FileName;
                        _rhinoCommOut.ShowMessage($"\nCONFIG FILE SELECTED: {Path.GetFileName(configPath)}");
                    }
                    else
                    {
                        _rhinoCommOut.ShowError("CONFIGURATION SELECTION CANCELED.");
                    }
                }
                catch (Exception ex)
                {
                    dialogException = ex;
                    _rhinoCommOut.ShowError($"CONFIG SELECTION FAILED: {ex.Message}");
                }
            });

            staThread.SetApartmentState(ApartmentState.STA); // Ensure STA mode
            staThread.Start();
            staThread.Join(); // Wait for dialog to complete

            if (dialogException != null)
                return null;

            return configPath;
        }
    }
}
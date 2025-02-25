using System;
using System.Windows.Forms;
using Config.Interfaces;

namespace Config
{
    public class ConfigSelUI : IConfigSelUI
    {
        private readonly IConfigState _state;

        public ConfigSelUI(IConfigState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public string SelectConfigFile()
        {
            try
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Title = "Select Config JSON File";
                    dialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                    dialog.InitialDirectory = string.IsNullOrEmpty(_state.LastConfigPath)
                        ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                        : _state.LastConfigPath;

                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                    {
                        _state.LastConfigPath = Path.GetDirectoryName(dialog.FileName);
                        return dialog.FileName;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting config file: {ex.Message}");
                return null;
            }
        }
    }
}
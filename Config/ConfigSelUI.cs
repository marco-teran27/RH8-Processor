using System;
using System.Windows.Forms;
using Interfaces;

namespace Config
{
    public class ConfigSelUI : IConfigSelector
    {
        public string SelectConfigFile()
        {
            try
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Title = "Select Config JSON File";
                    dialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                    {
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
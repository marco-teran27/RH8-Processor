using System;
using Interfaces;
using Rhino;

namespace RhinoInt
{
    public class RhinoBatchServices : IRhinoBatchServices
    {
        private RhinoDoc _currentDoc;

        public bool OpenFile(string filePath)
        {
            try
            {
                // Disable redraw globally—no re-enable per user request
                RhinoApp.RunScript("_NoEcho _SetRedrawOff", false);

                /// Opens file with redraw off—Rhino 8 compatible
                _currentDoc = RhinoDoc.Open(filePath, out bool _);
                if (_currentDoc != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Failed to open {filePath}: {ex.Message}");
                _currentDoc = null;
                return false;
            }
        }

        public void CloseFile()
        {
            try
            {
                if (_currentDoc != null)
                {
                    _currentDoc.Modified = false; // No save prompt
                    /// Using Dispose for single-file closure—works with CloseAllFiles
                    _currentDoc.Dispose();
                    _currentDoc = null;
                }
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Failed to close file: {ex.Message}");
                _currentDoc = null;
            }
        }

        /// <summary>
        /// Originally closed all documents and exited Rhino—now opens a new blank document
        /// to clear processed files while keeping Rhino open for debugging.
        /// </summary>
        public void CloseAllFiles()
        {
            try
            {
                /// Updated: Replace RhinoApp.Exit with NewDocument—closes all, starts fresh
                RhinoApp.RunScript("_-New None", false); // No template, no echo
                RhinoApp.WriteLine("Opened new blank document—processed files cleared.");
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Failed to open new document: {ex.Message}");
            }
        }
    }
}
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
                RhinoApp.RunScript("_NoEcho _SetRedrawOff", false);
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
                    _currentDoc.Modified = false;
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

        public void CloseAllFiles()
        {
            try
            {
                /// Updated: Fixed syntax—use _-New None with proper spacing for Rhino 8
                RhinoApp.RunScript("-_New None", false); // Opens blank doc, closes others
                RhinoApp.WriteLine("Opened new blank document—processed files cleared.");
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Failed to open new document: {ex.Message}");
            }
        }
    }
}
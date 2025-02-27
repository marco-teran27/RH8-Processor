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
                _currentDoc = RhinoDoc.Open(filePath, out bool _);
                return _currentDoc != null;
            }
            catch (Exception)
            {
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
                    _currentDoc.Dispose(); // Release resources
                    _currentDoc = null;
                }
            }
            catch (Exception)
            {
                // Silent fail—cleanup best effort
                _currentDoc = null;
            }
        }
    }
}
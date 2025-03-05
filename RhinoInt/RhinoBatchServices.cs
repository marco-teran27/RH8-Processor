using System;
using System.Threading;
using Interfaces;
using Rhino;

namespace RhinoInt
{
    public class RhinoBatchServices : IRhinoBatchServices
    {
        private RhinoDoc _currentDoc;
        private readonly IRhinoCommOut _rhinoCommOut; // Inject for logging

        public RhinoBatchServices(IRhinoCommOut rhinoCommOut)
        {
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
        }

        public bool OpenFile(string filePath)
        {
            _rhinoCommOut.ShowMessage($"DEBUG: Attempting to open {filePath} at {DateTime.Now} on thread {Thread.CurrentThread.ManagedThreadId}");
            _rhinoCommOut.ShowMessage($"DEBUG: Is UI thread? {IsCurrentThreadRhinoUI()}");
            try
            {
                // Use command-line approach to ensure UI thread execution
                RhinoApp.RunScript("_-Open \"" + filePath + "\"", false);
                _currentDoc = RhinoDoc.ActiveDoc; // Set to active document after command
                _rhinoCommOut.ShowMessage($"DEBUG: OpenFile result for {filePath} at {DateTime.Now}: {(_currentDoc != null ? "Success" : "Failed")}");
                return _currentDoc != null;
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowMessage($"DEBUG: Failed to open {filePath} at {DateTime.Now} on thread {Thread.CurrentThread.ManagedThreadId}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: Inner exception: {ex.InnerException.Message}");
                }
                if (ex.StackTrace != null)
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: Stack trace: {ex.StackTrace}");
                }
                _currentDoc = null;
                return false;
            }
        }

        public void CloseFile()
        {
            _rhinoCommOut.ShowMessage($"DEBUG: Attempting to close file at {DateTime.Now} on thread {Thread.CurrentThread.ManagedThreadId}");
            try
            {
                if (_currentDoc != null)
                {
                    _currentDoc.Modified = false;
                    _currentDoc.Dispose();
                    _currentDoc = null;
                    _rhinoCommOut.ShowMessage($"DEBUG: File closed successfully at {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowMessage($"DEBUG: Failed to close file at {DateTime.Now} on thread {Thread.CurrentThread.ManagedThreadId}: {ex.Message}");
                _currentDoc = null;
            }
        }

        public void CloseAllFiles()
        {
            _rhinoCommOut.ShowMessage($"DEBUG: Closing all files at {DateTime.Now} on thread {Thread.CurrentThread.ManagedThreadId}");
            try
            {
                RhinoApp.RunScript("-_New None", false);
                _rhinoCommOut.ShowMessage($"DEBUG: All files closed, new blank document opened at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowMessage($"DEBUG: Failed to close all files at {DateTime.Now} on thread {Thread.CurrentThread.ManagedThreadId}: {ex.Message}");
            }
        }

        // Helper method to check if current thread is Rhino's UI thread (corrected)
        private bool IsCurrentThreadRhinoUI()
        {
            try
            {
                // Test if we can safely access RhinoDoc.ActiveDoc.Name (requires UI thread)
                string docName = RhinoDoc.ActiveDoc?.Name ?? "";
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
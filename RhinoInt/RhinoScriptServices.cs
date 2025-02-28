using System;
using System.IO;
using System.Threading;
using Interfaces;
using Rhino;
using Grasshopper.Kernel;
using Rhino.Runtime;
using Commons.Params;

namespace RhinoInt
{
    /// <summary>
    /// Handles execution and completion monitoring of Rhino scripts (.py, .gh, .ghx).
    /// Implements IRhinoScriptServices to integrate with BatchService.
    /// </summary>
    public class RhinoScriptServices : IRhinoScriptServices
    {
        // Holds the current Grasshopper document for .gh/.ghx scripts
        private GH_Document _ghDoc;

        /// <summary>
        /// Executes a script based on its file type (.py or .gh/.ghx).
        /// </summary>
        /// <param name="scriptPath">Full path to the script file (e.g., "C:\...\test.py").</param>
        /// <returns>True if script starts successfully, false if it fails.</returns>
        // Above: namespace RhinoInt, class RhinoScriptServices, private GH_Document _ghDoc
        /// <summary>
        /// Executes a script based on its file type (.py or .gh/.ghx).
        /// </summary>
        public bool RunScript(string scriptPath)
        {
            try
            {
                string scriptType = Path.GetExtension(scriptPath)?.TrimStart('.').ToLower();
                switch (scriptType)
                {
                    case "py":
                        PythonScript python = PythonScript.Create();
                        python.ExecuteScript("import scriptcontext; scriptcontext.doc.Strings.SetString('ScriptDone', None)");
                        /// Updated: Add _NoEcho to suppress Python engine output
                        return RhinoApp.RunScript($"_-NoEcho -RunPythonScript \"{scriptPath}\"", false);
                    case "gh":
                    case "ghx":
                        var docIO = new GH_DocumentIO();
                        if (!docIO.Open(scriptPath)) return false;
                        _ghDoc = docIO.Document;
                        if (_ghDoc == null) return false;
                        Grasshopper.Instances.DocumentServer.AddDocument(_ghDoc);
                        _ghDoc.Enabled = true;
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Monitors script completion based on its type (.py or .gh/.ghx).
        /// Uses custom flag for .py, solver state for .gh/.ghx.
        /// </summary>
        /// <returns>True if script completes, false if it times out or fails.</returns>
        public bool WaitForScriptCompletion()
        {
            try
            {
                // Get script type from the current ScriptPath (set by BatchService)
                string scriptType = Path.GetExtension(ScriptPath.Instance.FullPath)?.TrimStart('.').ToLower();

                switch (scriptType)
                {
                    case "py":
                        // Python: Poll for "ScriptDone" flag set by the script
                        for (int i = 0; i < 50; i++) // ~5s timeout with 100ms intervals
                        {
                            string done = RhinoDoc.ActiveDoc?.Strings.GetValue("ScriptDone");
                            if (done == "true") return true; // Script signaled completion
                            Thread.Sleep(100); // Wait briefly before next check
                        }
                        return false; // Timeout—script didn’t set flag
                    case "gh":
                    case "ghx":
                        // Grasshopper: Check solver state in active canvas
                        if (_ghDoc == null || Grasshopper.Instances.ActiveCanvas?.Document == null) return false;
                        for (int i = 0; i < 50; i++) // ~5s timeout with 100ms intervals
                        {
                            // Solver done when Enabled = false and state is PostProcess
                            if (!_ghDoc.Enabled && _ghDoc.SolutionState == GH_ProcessStep.PostProcess)
                                return true;
                            Thread.Sleep(100); // Wait briefly before next check
                        }
                        return false; // Timeout—solver didn’t finish
                    default:
                        // Unsupported script type—fail silently
                        return false;
                }
            }
            catch (Exception)
            {
                // Catch any errors (e.g., RhinoDoc null, Grasshopper crash)
                return false;
            }
            finally
            {
                // Cleanup Grasshopper document reference
                _ghDoc = null;
            }
        }
    }
}
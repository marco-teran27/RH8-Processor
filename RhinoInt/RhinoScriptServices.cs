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
        public bool RunScript(string scriptPath)
        {
            try
            {
                // Extract script type from file extension (e.g., "py", "gh")
                string scriptType = Path.GetExtension(scriptPath)?.TrimStart('.').ToLower();

                switch (scriptType)
                {
                    case "py":
                        // Python script: Reset completion flag and run via Rhino's command line
                        PythonScript python = PythonScript.Create();
                        // Clear any prior "ScriptDone" flag to ensure accurate completion check
                        python.ExecuteScript("import scriptcontext; scriptcontext.doc.Strings.SetString('ScriptDone', None)");
                        // Execute Python script using Rhino's RunScript command
                        return RhinoApp.RunScript($"-_RunScript \"{scriptPath}\"", true);
                    case "gh":
                    case "ghx":
                        // Grasshopper script: Load .gh/.ghx file into a new GH_Document
                        var docIO = new GH_DocumentIO();
                        // Open returns true if file loads successfully, false if not
                        if (!docIO.Open(scriptPath)) return false;
                        // Get the loaded document from docIO
                        _ghDoc = docIO.Document;
                        if (_ghDoc == null) return false;
                        // Add to Grasshopper's document server to make it active
                        Grasshopper.Instances.DocumentServer.AddDocument(_ghDoc);
                        // Enable solver to start execution
                        _ghDoc.Enabled = true;
                        return true;
                    default:
                        // Unsupported script type—fail silently
                        return false;
                }
            }
            catch (Exception)
            {
                // Catch any unexpected errors (e.g., file access, Rhino crash)
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
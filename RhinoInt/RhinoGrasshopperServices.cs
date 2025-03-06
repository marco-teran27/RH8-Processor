using System;
using System.IO;
using System.Threading;
using Interfaces;
using Rhino;
using Commons.Params;
using Grasshopper.Kernel;

namespace RhinoInt
{
    public class RhinoGrasshopperServices : IRhinoGrasshopperServices
    {
        public bool RunScript(CancellationToken ct)
        {
            try
            {
                string scriptPath = ScriptPath.Instance.FullPath;
                if (string.IsNullOrEmpty(scriptPath))
                {
                    RhinoApp.WriteLine("Error: Script path is invalid.");
                    return false;
                }
                if (!File.Exists(scriptPath))
                {
                    RhinoApp.WriteLine($"Error: Grasshopper script not found: {scriptPath}");
                    return false;
                }

                // Open the Grasshopper document
                var io = new GH_DocumentIO();
                if (!io.Open(scriptPath))
                {
                    RhinoApp.WriteLine($"Error: Failed to open Grasshopper file: {scriptPath}");
                    return false;
                }

                GH_Document doc = io.Document;
                if (doc == null)
                {
                    RhinoApp.WriteLine($"Error: Invalid Grasshopper document: {scriptPath}");
                    return false;
                }

                // Set ScriptDone to false before running
                RhinoDoc.ActiveDoc?.Strings.SetString("ScriptDone", "false");

                // Add the document to Grasshopper's DocumentServer
                Grasshopper.Instances.DocumentServer.AddDocument(doc);

                // Use a ManualResetEvent to wait for solution completion
                using var solutionComplete = new ManualResetEvent(false);
                bool isComplete = false;

                doc.SolutionEnd += (sender, e) =>
                {
                    string scriptDone = RhinoDoc.ActiveDoc?.Strings.GetValue("ScriptDone") ?? "false";
                    isComplete = scriptDone == "true";
                    solutionComplete.Set(); // Signal completion
                };

                // Run the Grasshopper solution synchronously
                doc.Enabled = true;
                doc.NewSolution(true); // True forces a full recalculation

                // Wait for the solution to complete or cancellation
                solutionComplete.WaitOne(1000); // 1-second timeout as a fallback, but BatchService handles real timeout

                if (ct.IsCancellationRequested)
                {
                    RhinoApp.WriteLine("Grasshopper script cancelled.");
                    return false;
                }

                if (!isComplete)
                {
                    RhinoApp.WriteLine("Warning: Grasshopper script didn't signal completion (ScriptDone not set to 'true').");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Grasshopper script execution error at {DateTime.Now}: {ex.Message}");
                return false;
            }
        }
    }
}
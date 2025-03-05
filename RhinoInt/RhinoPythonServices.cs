using System;
using System.IO;
using System.Threading;
using Interfaces;
using Rhino;
using Commons.Params;
using Rhino.Runtime;

namespace RhinoInt
{
    public class RhinoPythonServices : IRhinoPythonServices
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
                    RhinoApp.WriteLine($"Error: Python script not found: {scriptPath}");
                    return false;
                }

                RhinoApp.WriteLine($"Starting Python script execution at {DateTime.Now}: {scriptPath}");

                // Initialize Python engine and execute directly
                PythonScript python = PythonScript.Create();
                python.ExecuteScript("import scriptcontext; scriptcontext.doc.Strings.SetString('ScriptDone', 'false')"); // Reset flag
                python.ExecuteFile(scriptPath);

                RhinoApp.WriteLine($"Python script execution completed at {DateTime.Now}");

                // Check completion with delay
                for (int i = 0; i < 10; i++) // 1-second timeout
                {
                    string scriptDone = RhinoDoc.ActiveDoc?.Strings.GetValue("ScriptDone") ?? "false";
                    if (scriptDone == "true")
                    {
                        RhinoApp.WriteLine($"Python script completion check at {DateTime.Now}: ScriptDone = {scriptDone}");
                        return true;
                    }
                    Thread.Sleep(100);
                }

                RhinoApp.WriteLine($"Python script completion check at {DateTime.Now}: ScriptDone = false (timeout)");
                return false;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Script execution error at {DateTime.Now}: {ex.Message}");
                return false;
            }
        }
    }
}
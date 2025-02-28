using System.IO;
using Commons;
using Commons.Interfaces;
using Commons.Utils;

namespace Commons.Params
{
    public class ScriptPath
    {
        private static readonly ScriptPath _instance = new ScriptPath();
        private string _fullPath;
        private string _scriptDir; // Added to store directory only

        private ScriptPath()
        {
            _fullPath = string.Empty;
            _scriptDir = string.Empty; // Initialize new field
        }

        public static ScriptPath Instance => _instance;

        public void SetScriptPath(IScriptSettings scriptSettings, IDirectorySettings directories)
        {
            string extension = scriptSettings.ScriptType switch
            {
                ScriptType.Python => ".py",
                ScriptType.Grasshopper => ".gh",
                ScriptType.GrasshopperXml => ".ghx",
                _ => ""
            };
            /// Updated: Store scriptDir separately—used for validation output
            _scriptDir = directories.ScriptDir;
            _fullPath = Path.Combine(directories.ScriptDir, $"{scriptSettings.ScriptName}{extension}");
        }

        public string FullPath => _fullPath;
        /// Updated: New property to access script directory only
        public string ScriptDir => _scriptDir;
    }
}
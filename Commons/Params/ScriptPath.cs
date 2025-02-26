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

        private ScriptPath()
        {
            _fullPath = string.Empty;
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
            _fullPath = Path.Combine(directories.ScriptDir, $"{scriptSettings.ScriptName}{extension}");
        }

        public string FullPath => _fullPath;
    }
}
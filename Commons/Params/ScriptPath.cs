using Interfaces;

namespace Commons.Params
{
    public class ScriptPath
    {
        private static readonly ScriptPath _instance = new ScriptPath();
        private string _fullPath = string.Empty;
        private string _scriptDir = string.Empty;

        private ScriptPath() { }

        public static ScriptPath Instance => _instance;

        public void SetScriptPath(IConfigDataResults config)
        {
            string extension = config.ScriptType.ToLower() switch
            {
                "python" => ".py",
                "grasshopper" => ".gh",
                "grasshopperxml" => ".ghx",
                _ => ""
            };
            _scriptDir = config.ScriptDir;
            _fullPath = System.IO.Path.Combine(_scriptDir, $"{config.ScriptName}{extension}");
        }

        public string FullPath => _fullPath;
        public string ScriptDir => _scriptDir;
    }
}
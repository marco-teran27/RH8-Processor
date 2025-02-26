using System.Collections.Generic;

namespace Commons.Params
{
    public class RhinoFileNameList
    {
        private static readonly RhinoFileNameList _instance = new RhinoFileNameList();
        private List<string> _matchedFiles;

        private RhinoFileNameList()
        {
            _matchedFiles = new List<string>();
        }

        public static RhinoFileNameList Instance => _instance;

        public void SetFiles(IEnumerable<string> files)
        {
            _matchedFiles.Clear();
            _matchedFiles.AddRange(files);
        }

        public IReadOnlyList<string> GetMatchedFiles() => _matchedFiles.AsReadOnly();
    }
}
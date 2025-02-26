using System.Collections.Generic;

namespace Commons.Params
{
    public class RhinoFileStatus
    {
        public string Id { get; } // Changed from FileName to Id
        public bool IsMatched { get; }

        public RhinoFileStatus(string id, bool isMatched)
        {
            Id = id;
            IsMatched = isMatched;
        }

        public override string ToString() => $"{Id}: {(IsMatched ? "matched" : "unmatched")}";
    }

    public class RhinoFileNameListLog
    {
        private static readonly RhinoFileNameListLog _instance = new RhinoFileNameListLog();
        private List<RhinoFileStatus> _idStatuses;

        private RhinoFileNameListLog()
        {
            _idStatuses = new List<RhinoFileStatus>();
        }

        public static RhinoFileNameListLog Instance => _instance;

        public void SetFiles(IEnumerable<RhinoFileStatus> idStatuses)
        {
            _idStatuses.Clear();
            _idStatuses.AddRange(idStatuses);
        }

        public IReadOnlyList<RhinoFileStatus> GetFileStatuses() => _idStatuses.AsReadOnly();
    }
}
using System.Collections.Generic;

/// ***NOTE FOR FUTURE DEV*** we need to consider if csv format should be processed here or downstream by a csv formatter

namespace Commons.LogFile
{
    public class PIDStatus
    {
        public string PID { get; }
        public bool IsValid { get; }

        public PIDStatus(string pid, bool isValid)
        {
            PID = pid;
            IsValid = isValid;
        }

        public override string ToString() => $"{PID}: {(IsValid ? "valid" : "invalid")}";
    }

    public class PIDListLog
    {
        private static readonly PIDListLog _instance = new PIDListLog();
        private List<PIDStatus> _pids;

        private PIDListLog()
        {
            _pids = new List<PIDStatus>();
        }

        public static PIDListLog Instance => _instance;

        public void SetPids(List<PIDStatus> pids)
        {
            _pids.Clear();
            _pids.AddRange(pids);
        }

        public IReadOnlyList<PIDStatus> GetPids() => _pids.AsReadOnly();

        public IEnumerable<PIDStatus> GetInvalidPids() => _pids.FindAll(p => !p.IsValid);
    }
}
using System.Collections.Generic;

namespace Commons.Params
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

    public class PIDListFormat
    {
        private static readonly PIDListFormat _instance = new PIDListFormat();
        private List<PIDStatus> _pids;

        private PIDListFormat()
        {
            _pids = new List<PIDStatus>();
        }

        public static PIDListFormat Instance => _instance;

        public void SetPids(List<PIDStatus> pids)
        {
            _pids.Clear();
            _pids.AddRange(pids);
        }

        public IReadOnlyList<PIDStatus> GetPids() => _pids.AsReadOnly();

        public IEnumerable<PIDStatus> GetInvalidPids() => _pids.FindAll(p => !p.IsValid);
    }
}
using System.Collections.Generic;
using System.Linq;
using Commons.Interfaces;
using Commons.Utils;
using System.Text.RegularExpressions;

namespace Commons.Params
{
    public class PIDList
    {
        private static readonly PIDList _instance = new PIDList();
        private List<string> _uniqueIds;

        private PIDList()
        {
            _uniqueIds = new List<string>();
        }

        public static PIDList Instance => _instance;

        public void CompileIds(IPIDSettings pidSettings, IRhinoFileNameSettings rhinoSettings)
        {
            _uniqueIds.Clear();

            if (pidSettings.Mode.Equals("all", System.StringComparison.OrdinalIgnoreCase) &&
                rhinoSettings.Mode.Equals("all", System.StringComparison.OrdinalIgnoreCase))
            {
                _uniqueIds.Add("*"); // All files in directory
            }
            else if (pidSettings.Mode.Equals("all", System.StringComparison.OrdinalIgnoreCase))
            {
                _uniqueIds.AddRange(rhinoSettings.Keywords); // Keywords only
            }
            else if (rhinoSettings.Mode.Equals("all", System.StringComparison.OrdinalIgnoreCase))
            {
                var validPids = PIDListLog.Instance.GetPids().Where(p => p.IsValid).Select(p => p.PID);
                foreach (var pid in validPids)
                {
                    var match = PatientIDRegex.Pattern.Match(pid);
                    if (match.Success)
                    {
                        string prefix = match.Groups[1].Value; // e.g., "300000L"
                        string suffix = match.Groups[2].Value; // e.g., "S12345"
                        _uniqueIds.Add($"{prefix}-*-{suffix}"); // Wildcard keyword
                    }
                }
            }
            else
            {
                var validPids = PIDListLog.Instance.GetPids().Where(p => p.IsValid).Select(p => p.PID);
                foreach (var pid in validPids)
                {
                    var match = PatientIDRegex.Pattern.Match(pid);
                    if (match.Success)
                    {
                        string prefix = match.Groups[1].Value; // e.g., "300000L"
                        string suffix = match.Groups[2].Value; // e.g., "S12345"
                        foreach (var keyword in rhinoSettings.Keywords)
                        {
                            _uniqueIds.Add($"{prefix}-{keyword}-{suffix}");
                        }
                    }
                }
            }
        }

        public IReadOnlyList<string> GetUniqueIds() => _uniqueIds.AsReadOnly();
    }
}
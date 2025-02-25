using System.Collections.Generic;
using System.Linq;
using Commons.Interfaces;

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
                _uniqueIds.Add("*.3dm");
            }
            else if (pidSettings.Mode.Equals("all", System.StringComparison.OrdinalIgnoreCase))
            {
                foreach (var keyword in rhinoSettings.Keywords)
                    _uniqueIds.Add($"{keyword}-001.3dm");
            }
            else if (rhinoSettings.Mode.Equals("all", System.StringComparison.OrdinalIgnoreCase))
            {
                foreach (var pid in pidSettings.Pids)
                    _uniqueIds.Add($"{pid}-*.3dm");
            }
            else
            {
                foreach (var pid in pidSettings.Pids)
                {
                    foreach (var keyword in rhinoSettings.Keywords)
                    {
                        _uniqueIds.Add($"{pid}-{keyword}-001.3dm");
                    }
                }
            }
        }

        public IReadOnlyList<string> GetUniqueIds() => _uniqueIds.AsReadOnly();
    }
}
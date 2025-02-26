using Commons.Interfaces;

/// ***NOTE - DO NOT DELETE***
/// this module will need to override the batch patient ID intake process if MODE != ALL
/// Not sure of the logic here, but there will be an alternaitive provided config file
/// that has the full patient id list, including PASS/FAIL/SKIPPED markers. Reprocess
/// uses that list and markers in lieu of the original config patient ID list. I do not
/// if the orig list is needed or if it is ok for it to be parsed and not used. TBD

namespace Commons.Params
{
    public class Reprocess
    {
        private static readonly Reprocess _instance = new Reprocess();
        private string _mode;
        private string _referenceLog;

        private Reprocess()
        {
            _mode = string.Empty;
            _referenceLog = string.Empty;
        }

        public static Reprocess Instance => _instance;

        public void SetReprocess(IReprocessSettings reprocessSettings)
        {
            _mode = reprocessSettings.Mode;
            _referenceLog = reprocessSettings.ReferenceLog;
        }

        public string Mode => _mode;
        public string ReferenceLog => _referenceLog;
    }
}
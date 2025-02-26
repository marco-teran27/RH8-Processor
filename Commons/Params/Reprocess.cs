using Commons.Interfaces;

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
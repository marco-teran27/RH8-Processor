using Commons.Interfaces;


/// ***FUTURE DEV NOTE*** do not delete this. timeout.min is a fail safe for frozen scripts. after 3-5 scripts that run
/// the timeout manager will avg script completion time and use that time*50% as the time out variable. 
/// If the first batch, the ones that are avg, do not complete before the config.timeout.min and the are skipped. batch
/// needs to close so the user can review the script on a single file. 

namespace Commons.Params
{
    public class TimeOutMin
    {
        private static readonly TimeOutMin _instance = new TimeOutMin();
        private int _minutes;

        private TimeOutMin()
        {
            _minutes = 0;
        }

        public static TimeOutMin Instance => _instance;

        public void SetMinutes(ITimeOutSettings timeoutSettings)
        {
            _minutes = timeoutSettings.Minutes;
        }

        public int Minutes => _minutes;
    }
}
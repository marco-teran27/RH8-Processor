using Commons.Interfaces;

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
using System.Collections.Generic;

namespace Commons.Interfaces
{
    public interface IPIDSettings
    {
        string Mode { get; }
        List<string> Pids { get; }
    }
}
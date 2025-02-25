using System.Collections.Generic;

namespace Commons.Interfaces
{
    public interface IRhinoFileNameSettings
    {
        string Mode { get; }
        List<string> Keywords { get; }
    }
}
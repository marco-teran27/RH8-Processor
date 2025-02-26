using System.Collections.Generic;
using Commons.Interfaces;
using Config.Models;

namespace Config.Interfaces
{
    public interface IValidator
    {
        (bool isValid, IReadOnlyList<string> messages) ValidateConfig(
            ProjectName projectName, // Kept as concrete type since no interface yet
            IDirectorySettings directories,
            IPIDSettings pidSettings,
            IRhinoFileNameSettings rhinoFileNameSettings,
            IScriptSettings scriptSettings,
            IReprocessSettings reprocessSettings,
            ITimeOutSettings timeoutSettings);
    }
}
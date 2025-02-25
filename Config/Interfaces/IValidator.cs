using System.Collections.Generic;

namespace Config.Interfaces
{
    public interface IValidator
    {
        (bool isValid, IReadOnlyList<string> messages) ValidateConfig(
            Models.ProjectName projectName,
            Models.DirectorySettings directories,
            Models.PIDSettings pidSettings,
            Models.RhinoFileNameSettings rhinoFileNameSettings,
            Models.ScriptSettings scriptSettings,
            Models.ReprocessSettings reprocessSettings,
            Models.TimeOutSettings timeoutSettings);
    }
}
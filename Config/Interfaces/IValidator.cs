// Config\Interfaces\IValidator.cs
namespace Config.Interfaces
{
    public interface IValidator
    {
        (bool isValid, string errorMessage) ValidateConfig(
            Models.ProjectName projectName,
            Models.DirectorySettings directories,
            Models.PIDSettings pidSettings,
            Models.RhinoFileNameSettings rhinoFileNameSettings,
            Models.ScriptSettings scriptSettings,
            Models.ReprocessSettings reprocessSettings,
            Models.TimeOutSettings timeoutSettings);
    }
}
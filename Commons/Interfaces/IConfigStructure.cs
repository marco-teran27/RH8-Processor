namespace Commons.Interfaces
{
    public interface IConfigStructure
    {
        string ProjectName { get; }
        IDirectorySettings Directories { get; }
        IPIDSettings PIDSettings { get; }
        IRhinoFileNameSettings RhinoFileNameSettings { get; }
        IScriptSettings ScriptSettings { get; }
        IReprocessSettings ReprocessSettings { get; }
        ITimeOutSettings TimeoutSettings { get; }
    }
}
namespace Commons.Interfaces
{
    public interface IDirectorySettings
    {
        string FileDir { get; }
        string OutputDir { get; }
        string ScriptDir { get; } // Included for ScriptPath
    }
}
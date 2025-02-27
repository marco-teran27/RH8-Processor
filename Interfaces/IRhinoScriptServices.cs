namespace Interfaces
{
    public interface IRhinoScriptServices
    {
        bool RunScript(string scriptPath);
        bool WaitForScriptCompletion();
    }
}
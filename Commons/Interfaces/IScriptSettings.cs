using Commons;
using Commons.Utils;

namespace Commons.Interfaces
{
    public interface IScriptSettings
    {
        string ScriptName { get; }
        ScriptType ScriptType { get; }
    }
}
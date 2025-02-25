// Config\ConfigState.cs
using Config.Interfaces;

namespace Config
{
    public class ConfigState : IConfigState
    {
        public string LastConfigPath { get; set; }
    }
}
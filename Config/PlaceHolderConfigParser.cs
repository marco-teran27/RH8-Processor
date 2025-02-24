using System.Threading.Tasks;
using Interfaces;

namespace Config
{
    public class PlaceholderConfigParser : IConfigParser
    {
        public Task<object> ParseConfigAsync(string configPath)
        {
            return Task.FromResult<object>($"Parsed {configPath}");
        }
    }
}
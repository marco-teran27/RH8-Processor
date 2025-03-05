using System.Threading.Tasks;
using Interfaces;

namespace Config.Interfaces
{
    public interface IConfigParser
    {
        Task<(IConfigDataResults Data, IConfigValResults Val)> ParseConfigAsync(string configPath); // Async
    }
}
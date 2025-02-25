using System.Threading.Tasks;
using Commons;

namespace Config.Interfaces
{
    public interface IConfigParser
    {
        Task<ConfigValidationResults> ParseConfigAsync(string configPath);
    }
}
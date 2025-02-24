using System.Threading.Tasks;

namespace Interfaces
{
    public interface IConfigParser
    {
        Task<object> ParseConfigAsync(string configPath); // Adjust return type later
    }
}
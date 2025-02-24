using System.Threading;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ITheOrchestrator
    {
        Task<bool> RunBatchAsync(string? configPath, CancellationToken ct);
    }
}
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IBatchService
    {
        Task RunBatchAsync(CancellationToken ct);
        /// <summary>
        /// Closes all open Rhino files—ensures cleanup after batch execution.
        /// Added to delegate file closure to RhinoBatchServices.
        /// </summary>
        void CloseAllFiles();
    }
}
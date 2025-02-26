using System.Threading;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface IFileDirScanner
    {
        Task ScanAsync(CancellationToken ct);
    }
}
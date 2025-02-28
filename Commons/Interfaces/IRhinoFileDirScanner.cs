using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface IRhinoFileDirScanner
    {
        Task ScanAsync(CancellationToken ct);
        /// Updated: Add validation method
        ValidationResult Validate();
    }
}
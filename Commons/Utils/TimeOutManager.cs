using System;
using System.Threading;
using System.Threading.Tasks;

namespace Commons.Utils
{
    public static class TimeOutManager
    {
        public static async Task<bool> RunWithTimeoutAsync(Func<Task> action, int timeOutMinutes, CancellationToken ct)
        {
            var timeoutMs = timeOutMinutes * 60 * 1000;
            var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(timeoutMs);

            try
            {
                await action();
                return true;
            }
            catch (OperationCanceledException)
            {
                return false; // Timed out
            }
            catch (Exception)
            {
                throw; // Other errors propagate
            }
            finally
            {
                cts.Dispose();
            }
        }
    }
}
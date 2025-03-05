using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Interfaces;
using Commons.Params;
using Commons.LogFile;
using Commons.LogComm;

namespace Core.Batch
{
    public class BatchService : IBatchService
    {
        private readonly IRhinoCommOut _rhinoCommOut;
        private readonly IRhinoBatchServices _batchServices;
        private readonly IRhinoPythonServices _scriptServices;

        public BatchService(
            IRhinoCommOut rhinoCommOut,
            IRhinoBatchServices batchServices,
            IRhinoPythonServices scriptServices)
        {
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
            _batchServices = batchServices ?? throw new ArgumentNullException(nameof(batchServices));
            _scriptServices = scriptServices ?? throw new ArgumentNullException(nameof(scriptServices));
        }

        public async Task RunBatchAsync(CancellationToken ct)
        {
            try
            {
                var files = RhinoFileNameList.Instance.GetMatchedFiles();
                if (!files.Any())
                {
                    _rhinoCommOut.ShowError("No matched files found");
                    return;
                }

                foreach (var file in files)
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        bool success = await Task.Run(async () =>
                        {
                            if (!_batchServices.OpenFile(file))
                            {
                                _rhinoCommOut.ShowError($"Failed to open {Path.GetFileName(file)}");
                                return false;
                            }

                            bool scriptSuccess = await Task.Run(() => _scriptServices.RunScript(ct));
                            if (!scriptSuccess)
                            {
                                _rhinoCommOut.ShowError($"Python script failed to execute for {Path.GetFileName(file)}");
                            }

                            _batchServices.CloseFile();
                            return scriptSuccess;
                        }, ct).TimeoutAfter(TimeSpan.FromSeconds(30));

                        BatchServiceLog.Instance.AddStatus(file, success ? "PASS" : "FAIL");
                    }
                    catch (TimeoutException)
                    {
                        _rhinoCommOut.ShowError($"TIMEOUT processing {Path.GetFileName(file)}");
                        BatchServiceLog.Instance.AddStatus(file, "FAIL");
                        _batchServices.CloseFile();
                    }
                    catch (Exception ex)
                    {
                        _rhinoCommOut.ShowError($"Error processing {Path.GetFileName(file)}: {ex.Message}");
                        BatchServiceLog.Instance.AddStatus(file, "FAIL");
                        _batchServices.CloseFile();
                    }
                }
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"Batch failed: {ex.Message}");
            }
            finally
            {
                CloseAllFiles();
            }
        }

        public void CloseAllFiles()
        {
            _batchServices.CloseAllFiles();
        }
    }

    public static class TaskExtensions
    {
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource())
            {
                var delayTask = Task.Delay(timeout, cts.Token);
                var completedTask = await Task.WhenAny(task, delayTask);
                if (completedTask == delayTask)
                    throw new TimeoutException("Operation timed out.");
                cts.Cancel();
                return await task;
            }
        }
    }
}
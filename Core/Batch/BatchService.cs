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
        private readonly IRhinoPythonServices _scriptServices; // Added for script execution

        public BatchService(
            IRhinoCommOut rhinoCommOut,
            IRhinoBatchServices batchServices,
            IRhinoPythonServices scriptServices) // Inject RhinoScriptServices
        {
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
            _batchServices = batchServices ?? throw new ArgumentNullException(nameof(batchServices));
            _scriptServices = scriptServices ?? throw new ArgumentNullException(nameof(scriptServices));
        }

        public async Task RunBatchAsync(CancellationToken ct)
        {
            _rhinoCommOut.ShowMessage($"DEBUG: Starting RunBatchAsync at {DateTime.Now}");
            try
            {
                var files = RhinoFileNameList.Instance.GetMatchedFiles();
                if (!files.Any())
                {
                    _rhinoCommOut.ShowError($"DEBUG: No matched files found at {DateTime.Now}");
                    return;
                }

                _rhinoCommOut.ShowMessage($"DEBUG: Starting batch processing of {files.Count} files at {DateTime.Now}");

                foreach (var file in files)
                {
                    if (ct.IsCancellationRequested)
                    {
                        _rhinoCommOut.ShowMessage($"DEBUG: Cancellation requested at {DateTime.Now} for file {Path.GetFileName(file)}");
                        break;
                    }

                    _rhinoCommOut.ShowMessage($"DEBUG: Attempting to process {Path.GetFileName(file)} at {DateTime.Now}");
                    try
                    {
                        // Use Task.Run to offload to thread pool, with a 30-second timeout
                        bool success = await Task.Run(async () =>
                        {
                            if (!_batchServices.OpenFile(file))
                            {
                                _rhinoCommOut.ShowError($"DEBUG: Failed to open {Path.GetFileName(file)} at {DateTime.Now}");
                                return false;
                            }

                            _rhinoCommOut.ShowMessage($"DEBUG: File {Path.GetFileName(file)} opened at {DateTime.Now}");

                            // Ensure script execution on UI thread with timeout
                            _rhinoCommOut.ShowMessage($"DEBUG: Attempting to run script for {Path.GetFileName(file)} at {DateTime.Now}");
                            bool scriptSuccess = await Task.Run(() => _scriptServices.RunScript(ct)); // UI thread via RhinoScriptServices
                            _rhinoCommOut.ShowMessage($"DEBUG: Script execution for {Path.GetFileName(file)} at {DateTime.Now}: {scriptSuccess}");

                            if (!scriptSuccess)
                            {
                                _rhinoCommOut.ShowError($"Python script failed to execute for {Path.GetFileName(file)} at {DateTime.Now}");
                            }

                            _batchServices.CloseFile();
                            return scriptSuccess; // Return script success for overall status
                        }, ct).TimeoutAfter(TimeSpan.FromSeconds(30));

                        BatchServiceLog.Instance.AddStatus(file, success ? "PASS" : "FAIL");

                        int index = files.ToList().FindIndex(f => f == file);
                        if (index % 10 == 0 || index == files.Count - 1)
                        {
                            _rhinoCommOut.ShowMessage($"DEBUG: Processed {index + 1} files at {DateTime.Now}");
                        }
                    }
                    catch (TimeoutException)
                    {
                        _rhinoCommOut.ShowError($"DEBUG: TIMEOUT processing {Path.GetFileName(file)} at {DateTime.Now}");
                        BatchServiceLog.Instance.AddStatus(file, "FAIL");
                        _batchServices.CloseFile();
                    }
                    catch (Exception ex)
                    {
                        _rhinoCommOut.ShowError($"DEBUG: Error processing {Path.GetFileName(file)} at {DateTime.Now}: {ex.Message}");
                        BatchServiceLog.Instance.AddStatus(file, "FAIL");
                        _batchServices.CloseFile();
                    }
                }

                _rhinoCommOut.ShowMessage($"DEBUG: Batch processing completed at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"DEBUG: Batch failed at {DateTime.Now}: {ex.Message}");
            }
            finally
            {
                CloseAllFiles();
            }
        }

        public void CloseAllFiles()
        {
            _rhinoCommOut.ShowMessage($"DEBUG: Closing all files at {DateTime.Now}");
            _batchServices.CloseAllFiles();
        }
    }

    // Extension method for timeout
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
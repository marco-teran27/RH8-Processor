using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Interfaces;
using Commons.Params;
using Commons.Utils;
using Commons.Logging;

namespace Core.Batch
{
    public class BatchService : IBatchService
    {
        private readonly IRhinoCommOut _rhinoCommOut;
        private readonly IRhinoBatchServices _batchServices;
        private readonly IRhinoScriptServices _scriptServices;

        public BatchService(
            IRhinoCommOut rhinoCommOut,
            IRhinoBatchServices batchServices,
            IRhinoScriptServices scriptServices)
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
                    _rhinoCommOut.ShowError("No matched files found in RhinoFileNameList.");
                    return;
                }

                _rhinoCommOut.ShowMessage($"Starting batch processing of {files.Count} files...");
                var processedFiles = new List<string>();
                int timeoutMinutes = TimeOutMin.Instance.Minutes;

                foreach (var file in files)
                {
                    if (ct.IsCancellationRequested) break;

                    try
                    {
                        _rhinoCommOut.ShowMessage($"Processing: {file}");
                        if (!_batchServices.OpenFile(file))
                        {
                            _rhinoCommOut.ShowError($"Failed to open {file}. Skipping.");
                            BatchServiceLog.Instance.AddStatus(file, "FAIL");
                            _batchServices.CloseFile();
                            continue;
                        }

                        string scriptPath = ScriptPath.Instance.FullPath;
                        if (string.IsNullOrEmpty(scriptPath))
                        {
                            _rhinoCommOut.ShowError($"Script path invalid. Skipping {file}.");
                            BatchServiceLog.Instance.AddStatus(file, "FAIL");
                            _batchServices.CloseFile();
                            continue;
                        }

                        bool scriptSuccess = await TimeOutManager.RunWithTimeoutAsync(
                            async () =>
                            {
                                if (!_scriptServices.RunScript(scriptPath))
                                    throw new Exception("Script execution failed.");
                                if (!_scriptServices.WaitForScriptCompletion())
                                    throw new Exception("Script did not complete.");
                                await Task.CompletedTask;
                            },
                            timeoutMinutes,
                            ct);

                        if (!scriptSuccess)
                        {
                            _rhinoCommOut.ShowError($"Script failed or timed out on {file}. Skipping.");
                            BatchServiceLog.Instance.AddStatus(file, "FAIL");
                        }
                        else
                        {
                            _rhinoCommOut.ShowMessage($"{file}: complete");
                            BatchServiceLog.Instance.AddStatus(file, "PASS");
                        }

                        _batchServices.CloseFile();
                        processedFiles.Add(file);

                        if (processedFiles.Count % 10 == 0)
                        {
                            _rhinoCommOut.ShowMessage($"Processed {processedFiles.Count} files\nEstimated completion time: TBD");
                        }
                    }
                    catch (Exception ex)
                    {
                        _rhinoCommOut.ShowError($"Error processing {file}: {ex.Message}. Skipping.");
                        BatchServiceLog.Instance.AddStatus(file, "FAIL");
                        _batchServices.CloseFile();
                    }
                }

                if (processedFiles.Count % 10 != 0)
                {
                    _rhinoCommOut.ShowMessage($"Processed {processedFiles.Count} files\nEstimated completion time: TBD");
                }
                _rhinoCommOut.ShowMessage("Batch processing completed.");
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"Batch failed: {ex.Message}");
            }
        }
    }
}
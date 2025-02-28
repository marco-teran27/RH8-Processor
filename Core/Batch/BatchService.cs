using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Interfaces;
using Commons.Params;
using Commons.Utils;
using Commons.LogFile;
using Commons.LogComm;

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
                    _rhinoCommOut.ShowError("NO MATCHED FILES FOUND IN RHINOFILENAMELIST.");
                    return;
                }

                _rhinoCommOut.ShowMessage($"STARTING BATCH PROCESSING OF {files.Count} FILES...");
                var processedFiles = new List<string>();
                int timeoutMinutes = TimeOutMin.Instance.Minutes;

                foreach (var file in files)
                {
                    if (ct.IsCancellationRequested) break;

                    try
                    {
                        _rhinoCommOut.ShowMessage($"Processing: {Path.GetFileName(file)}");
                        if (!_batchServices.OpenFile(file))
                        {
                            _rhinoCommOut.ShowError($"FAILED TO OPEN {Path.GetFileName(file)}. SKIPPING.");
                            BatchServiceLog.Instance.AddStatus(file, "FAIL");
                            _batchServices.CloseFile();
                            continue;
                        }

                        string scriptPath = ScriptPath.Instance.FullPath;
                        if (string.IsNullOrEmpty(scriptPath))
                        {
                            _rhinoCommOut.ShowError($"SCRIPT PATH INVALID. SKIPPING {Path.GetFileName(file)}.");
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
                            _rhinoCommOut.ShowError($"SCRIPT FAILED OR TIMED OUT ON {Path.GetFileName(file)}. SKIPPING.");
                            BatchServiceLog.Instance.AddStatus(file, "FAIL");
                        }
                        else
                        {
                            BatchServiceLog.Instance.AddStatus(file, "PASS");
                        }

                        _batchServices.CloseFile();
                        processedFiles.Add(file);

                        if (processedFiles.Count % 10 == 0)
                        {
                            _rhinoCommOut.ShowMessage($"PROCESSED {processedFiles.Count} FILES\nESTIMATED COMPLETION TIME: TBD");
                        }
                    }
                    catch (Exception ex)
                    {
                        _rhinoCommOut.ShowError($"ERROR PROCESSING {Path.GetFileName(file)}: {ex.Message}. SKIPPING.");
                        BatchServiceLog.Instance.AddStatus(file, "FAIL");
                        _batchServices.CloseFile();
                    }
                }

                if (processedFiles.Count % 10 != 0)
                {
                    _rhinoCommOut.ShowMessage($"PROCESSED {processedFiles.Count} FILES\nESTIMATED COMPLETION TIME: TBD");
                }
                _rhinoCommOut.ShowMessage("BATCH PROCESSING COMPLETED.");
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"BATCH FAILED: {ex.Message}");
            }
            finally
            {
                CloseAllFiles();
            }
        }

        public void CloseAllFiles()
        {
            /// Updated: Move completion message to RhinoFileDirValComm
            _batchServices.CloseAllFiles();
        }
    }
}
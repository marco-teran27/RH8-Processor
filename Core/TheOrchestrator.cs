using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Config.Interfaces;
using Interfaces;
using Commons.LogComm;
using Commons.LogFile;
using FileDir;
using Commons.Params;

namespace Core
{
    public class TheOrchestrator : ITheOrchestrator
    {
        private readonly IConfigSelUI _selector;
        private readonly IConfigParser _parser;
        private readonly FileNameValComm _fileDirComm;
        private readonly IBatchService _batchService;
        private readonly IFileDirParser _fileDirParser;
        private readonly IRhinoCommOut _rhinoCommOut;
        private readonly ICommonsDataService _commonsDataService;

        public TheOrchestrator(
            IConfigSelUI selector,
            IConfigParser parser,
            FileNameValComm fileDirComm,
            IBatchService batchService,
            IFileDirParser fileDirParser,
            IRhinoCommOut rhinoCommOut,
            ICommonsDataService commonsDataService)
        {
            _selector = selector;
            _parser = parser;
            _fileDirComm = fileDirComm;
            _batchService = batchService;
            _fileDirParser = fileDirParser;
            _rhinoCommOut = rhinoCommOut;
            _commonsDataService = commonsDataService ?? throw new ArgumentNullException(nameof(commonsDataService));
        }

        public async Task<bool> RunBatchAsync(string? configPath, CancellationToken ct)
        {
            _rhinoCommOut.ShowMessage($"DEBUG: Starting RunBatchAsync at {DateTime.Now}");
            try
            {
                configPath ??= await Task.Run(() => _selector.SelectConfigFile(), ct);
                _rhinoCommOut.ShowMessage($"DEBUG: Config file selected: {configPath} at {DateTime.Now}");
                if (string.IsNullOrEmpty(configPath) || ct.IsCancellationRequested)
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: No valid config path or canceled at {DateTime.Now}");
                    return false;
                }

                _rhinoCommOut.ShowMessage($"DEBUG: Parsing config at {DateTime.Now}");
                var (dataResults, valResults) = await _parser.ParseConfigAsync(configPath);
                _rhinoCommOut.ShowMessage($"DEBUG: Config parsed, validating at {DateTime.Now}");
                ConfigValComm.LogValidationResults(valResults, _rhinoCommOut);

                if (!valResults.IsValid)
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: Validation failed at {DateTime.Now}");
                    return false;
                }

                _rhinoCommOut.ShowMessage($"DEBUG: Updating commons data at {DateTime.Now}");
                _commonsDataService.UpdateFromConfig(dataResults, valResults);

                _rhinoCommOut.ShowMessage($"DEBUG: Parsing file directory at {DateTime.Now}");
                (IFileNameList fileDirData, IFileNameValResults fileDirVal) = await _fileDirParser.ParseFileDirAsync(
                    dataResults.FileDir, PIDList.Instance.GetUniqueIds(), dataResults);
                if (fileDirData == null || fileDirVal == null)
                {
                    _rhinoCommOut.ShowError($"DEBUG: File parsing failed at {DateTime.Now}");
                    return false;
                }
                _rhinoCommOut.ShowMessage($"DEBUG: Updating file dir data at {DateTime.Now}");
                _commonsDataService.UpdateFromFileDir(fileDirData, fileDirVal);

                _rhinoCommOut.ShowMessage($"DEBUG: Setting logs at {DateTime.Now}");
                PIDListLog.Instance.SetPids(dataResults, valResults);
                FileNameListLog.Instance.SetFiles(dataResults, fileDirData);

                _rhinoCommOut.ShowMessage($"DEBUG: Logging validation and scan results at {DateTime.Now}");
                if (!_fileDirComm.LogValidationAndScanResults(fileDirVal, fileDirData.MatchedFiles.Count, dataResults.Pids.Count))
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: File dir validation failed at {DateTime.Now}");
                    return false;
                }

                _rhinoCommOut.ShowMessage($"DEBUG: Running batch at {DateTime.Now}");
                await _batchService.RunBatchAsync(ct); // Changed to async call
                _rhinoCommOut.ShowMessage($"DEBUG: Batch completed, logging completion at {DateTime.Now}");
                _fileDirComm.LogCompletion(true);
                return true;
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"DEBUG: RunBatchAsync failed at {DateTime.Now}: {ex.Message}");
                _fileDirComm.LogCompletion(false);
                return false;
            }
        }

        public bool RunBatch(string? configPath, CancellationToken ct)
        {
            _rhinoCommOut.ShowMessage($"DEBUG: Starting RunBatch at {DateTime.Now}");
            var task = Task.Run(() => RunBatchAsync(configPath, ct), ct);
            try
            {
                task.Wait(ct);
                _rhinoCommOut.ShowMessage($"DEBUG: RunBatch task completed at {DateTime.Now}");
                return task.Result;
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"DEBUG: RunBatch failed at {DateTime.Now}: {ex.Message}");
                _fileDirComm.LogCompletion(false);
                return false;
            }
        }
    }
}
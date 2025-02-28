using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Config;
using Config.Interfaces;
using Commons;
using Commons.Interfaces;
using Interfaces;
using Commons.LogComm;

namespace Core
{
    public class TheOrchestrator : ITheOrchestrator
    {
        private readonly IConfigSelUI _selector;
        private readonly IConfigParser _parser;
        private readonly IRhinoCommOut _rhinoCommOut;
        private readonly IFileDirScanner _fileDirScanner;
        private readonly IBatchService _batchService;

        public TheOrchestrator(
            IConfigSelUI selector,
            IConfigParser parser,
            IRhinoCommOut rhinoCommOut,
            IFileDirScanner fileDirScanner,
            IBatchService batchService)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
            _fileDirScanner = fileDirScanner ?? throw new ArgumentNullException(nameof(fileDirScanner));
            _batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
        }

        public async Task<bool> RunBatchAsync(string? configPath, CancellationToken ct)
        {
            try
            {
                /// Updated: Lowercase per request
                _rhinoCommOut.ShowMessage("\nstarting batchprocessor");
                configPath ??= _selector.SelectConfigFile();
                _rhinoCommOut.ShowMessage($"\nCONFIG FILE SELECTED: {Path.GetFileName(configPath)}");

                if (string.IsNullOrEmpty(configPath) || ct.IsCancellationRequested)
                {
                    _rhinoCommOut.ShowError("CONFIGURATION SELECTION CANCELED.");
                    return false;
                }

                var configResult = await _parser.ParseConfigAsync(configPath);
                ConfigValLog.LogValidationResults(configResult, _rhinoCommOut);

                if (!configResult.IsValid)
                {
                    /// Updated: Remove duplicate fail message—ConfigValLog handles it
                    //_rhinoCommOut.ShowError("VALIDATION FAILED. PLEASE ADDRESS THE ISSUES ABOVE.");
                    return false;
                }

                _rhinoCommOut.ShowMessage("\nRHINO FILE DIR");
                await _fileDirScanner.ScanAsync(ct);

                var matchedFiles = Commons.Params.RhinoFileNameList.Instance.GetMatchedFiles();
                int expectedCount = Commons.Params.PIDList.Instance.GetUniqueIds().Count;
                if (matchedFiles.Count == 0)
                {
                    _rhinoCommOut.ShowMessage($"0 OF {expectedCount} MATCHED\nNO FILES FOR BATCH EXECUTE");
                    return false;
                }

                _rhinoCommOut.ShowMessage($"{matchedFiles.Count} OF {expectedCount} MATCHED\nSTARTING BATCH EXECUTION");
                await _batchService.RunBatchAsync(ct);

                _rhinoCommOut.ShowMessage("BATCH PROCESS COMPLETED SUCCESSFULLY.");
                return true;
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"CONFIG PIPELINE FAILED: {ex.Message}");
                return false;
            }
            finally
            {
                _batchService.CloseAllFiles();
            }
        }
    }
}
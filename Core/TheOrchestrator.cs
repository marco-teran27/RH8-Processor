using System;
using System.IO; // Added for Path
using System.Threading;
using System.Threading.Tasks;
using Config;
using Config.Interfaces;
using Commons;
using Commons.Interfaces;
using Interfaces;

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
                _rhinoCommOut.ShowMessage("Starting BatchProcessor...");
                configPath ??= _selector.SelectConfigFile();
                /// Updated: Show only config filename—not full path
                _rhinoCommOut.ShowMessage($"Config file selected: {Path.GetFileName(configPath)}");

                if (string.IsNullOrEmpty(configPath) || ct.IsCancellationRequested)
                {
                    _rhinoCommOut.ShowError("Configuration selection canceled.");
                    return false;
                }

                _rhinoCommOut.ShowMessage("Parsing config file...");
                var configResult = await _parser.ParseConfigAsync(configPath);
                _rhinoCommOut.ShowMessage("Config parsing completed.");

                Commons.Logging.ConfigValLog.LogValidationResults(configResult, _rhinoCommOut);

                if (!configResult.IsValid)
                {
                    _rhinoCommOut.ShowError("Validation failed. Please address the issues above.");
                    return false;
                }

                /// Updated: Removed duplicate "All validations passed"—ConfigValLog handles it
                _rhinoCommOut.ShowMessage("RHINO FILE DIR");
                await _fileDirScanner.ScanAsync(ct);

                var matchedFiles = Commons.Params.RhinoFileNameList.Instance.GetMatchedFiles();
                int expectedCount = Commons.Params.PIDList.Instance.GetUniqueIds().Count;
                if (matchedFiles.Count == 0)
                {
                    _rhinoCommOut.ShowMessage($"0 of {expectedCount} matched\nNo Files for Batch Execute");
                    return false;
                }

                _rhinoCommOut.ShowMessage($"{matchedFiles.Count} of {expectedCount} matched\nStarting batch execution");
                await _batchService.RunBatchAsync(ct);

                _rhinoCommOut.ShowMessage("Batch process completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"Config pipeline failed: {ex.Message}");
                return false;
            }
            finally
            {
                _batchService.CloseAllFiles();
            }
        }
    }
}
using System;
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
                // If no config path provided, prompt user to select one
                configPath ??= _selector.SelectConfigFile();
                _rhinoCommOut.ShowMessage($"Config file selected: {configPath}");

                // Check if selection was canceled or invalid
                if (string.IsNullOrEmpty(configPath) || ct.IsCancellationRequested)
                {
                    _rhinoCommOut.ShowError("Configuration selection canceled.");
                    return false;
                }

                // Parse the config file into ConfigStructure and validate
                _rhinoCommOut.ShowMessage("Parsing config file...");
                var configResult = await _parser.ParseConfigAsync(configPath);
                _rhinoCommOut.ShowMessage("Config parsing completed.");

                // Log validation results (populates Commons.Params)
                Commons.Logging.ConfigValLog.LogValidationResults(configResult, _rhinoCommOut);

                // Exit if config is invalid
                if (!configResult.IsValid)
                {
                    _rhinoCommOut.ShowError("Validation failed. Please address the issues above.");
                    return false;
                }

                _rhinoCommOut.ShowMessage($"Config parsed and validated from {configPath}");

                // Pre-parse file directory for matching Rhino files
                _rhinoCommOut.ShowMessage("Starting file directory pre-parsing...");
                await _fileDirScanner.ScanAsync(ct);

                // Run batch processing on matched files
                _rhinoCommOut.ShowMessage("Starting batch execution...");
                await _batchService.RunBatchAsync(ct);

                _rhinoCommOut.ShowMessage("Batch process completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowError($"Config pipeline failed: {ex.Message}");
                return false;
            }
        }
    }
}
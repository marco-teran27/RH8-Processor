using System;
using System.Threading;
using System.Threading.Tasks;
using Config.Interfaces;
using Commons;
using Commons.Interfaces;
using Commons.LogComm;
using Interfaces;
using Commons.Params;

namespace Core
{
    public class TheOrchestrator : ITheOrchestrator
    {
        private readonly IConfigSelUI _selector;
        private readonly IConfigParser _parser;
        private readonly RhinoFileDirValComm _fileDirComm;
        private readonly IBatchService _batchService;
        private readonly IRhinoFileDirScanner _fileDirScanner; // Updated: Add scanner

        public TheOrchestrator(
            IConfigSelUI selector,
            IConfigParser parser,
            RhinoFileDirValComm fileDirComm,
            IBatchService batchService,
            IRhinoFileDirScanner fileDirScanner)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _fileDirComm = fileDirComm ?? throw new ArgumentNullException(nameof(fileDirComm));
            _batchService = batchService ?? throw new ArgumentNullException(nameof(batchService));
            _fileDirScanner = fileDirScanner ?? throw new ArgumentNullException(nameof(fileDirScanner));
        }

        public async Task<bool> RunBatchAsync(string? configPath, CancellationToken ct)
        {
            try
            {
                configPath ??= _selector.SelectConfigFile();

                if (string.IsNullOrEmpty(configPath) || ct.IsCancellationRequested)
                {
                    return false;
                }

                var configResult = await _parser.ParseConfigAsync(configPath);
                ConfigValComm.LogValidationResults(configResult, _selector.RhinoCommOut);

                if (!configResult.IsValid)
                {
                    return false;
                }

                /// Updated: Use _fileDirScanner directly instead of Validate() method
                var validationResult = _fileDirScanner.Validate();
                var matchedFiles = RhinoFileNameList.Instance.GetMatchedFiles();
                int expectedCount = PIDList.Instance.GetUniqueIds().Count;

                if (!_fileDirComm.LogValidationAndScanResults(validationResult, matchedFiles.Count, expectedCount))
                {
                    return false;
                }

                await _batchService.RunBatchAsync(ct);
                _fileDirComm.LogCompletion(true);
                return true;
            }
            catch (Exception ex)
            {
                _selector.RhinoCommOut.ShowError($"CONFIG PIPELINE FAILED: {ex.Message}");
                _fileDirComm.LogCompletion(false);
                return false;
            }
        }
    }
}
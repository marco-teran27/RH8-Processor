using System;
using System.Threading;
using System.Threading.Tasks;
using Config.Interfaces;
using Interfaces;
using Config;
using Commons; // Added for ConfigValidationResults

namespace Core
{
    public class TheOrchestrator : ITheOrchestrator
    {
        private readonly IConfigSelUI _selector;
        private readonly IConfigParser _parser;
        private readonly IRhinoCommOut _rhino;

        public TheOrchestrator(
            IConfigSelUI selector,
            IConfigParser parser,
            IRhinoCommOut rhino)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _rhino = rhino ?? throw new ArgumentNullException(nameof(rhino));
        }

        public async Task<bool> RunBatchAsync(string? configPath, CancellationToken ct)
        {
            try
            {
                _rhino.ShowMessage("Starting BatchProcessor...");
                configPath ??= _selector.SelectConfigFile();
                if (string.IsNullOrEmpty(configPath) || ct.IsCancellationRequested)
                {
                    _rhino.ShowError("Configuration selection canceled.");
                    return false;
                }

                var configResult = await _parser.ParseConfigAsync(configPath);
                if (!configResult.IsValid)
                {
                    foreach (var msg in configResult.Errors) // Changed from ValidationMessages
                        _rhino.ShowError(msg);
                    return false;
                }

                _rhino.ShowMessage($"Config parsed and validated from {configPath}"); // Changed from configResult.FilePath
                return true;
            }
            catch (Exception ex)
            {
                _rhino.ShowError($"Config pipeline failed: {ex.Message}");
                return false;
            }
        }
    }
}
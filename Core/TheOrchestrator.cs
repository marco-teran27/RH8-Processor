using System;
using System.Threading;
using System.Threading.Tasks;
using Config.Interfaces;
using Interfaces;
using Config;
using Commons;

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
                _rhino.ShowMessage($"Config file selected: {configPath}");
                if (string.IsNullOrEmpty(configPath) || ct.IsCancellationRequested)
                {
                    _rhino.ShowError("Configuration selection canceled.");
                    return false;
                }

                _rhino.ShowMessage("Parsing config file...");
                var configResult = await _parser.ParseConfigAsync(configPath);
                _rhino.ShowMessage("Config parsing completed.");

                if (!configResult.IsValid)
                {
                    foreach (var msg in configResult.Errors)
                        _rhino.ShowError(msg);
                    return false;
                }

                _rhino.ShowMessage($"Config parsed and validated from {configPath}");
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
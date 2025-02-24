using System;
using System.Threading;
using System.Threading.Tasks;
using Interfaces; // Replace Commons with Interfaces

namespace Core
{
    public class TheOrchestrator : ITheOrchestrator
    {
        private readonly IConfigSelector _selector; // From Config
        private readonly IConfigParser _parser;     // New interface, to be added
        private readonly IRhinoCommOut _rhino;      // New interface, to be added

        public TheOrchestrator(IConfigSelector selector, IConfigParser parser, IRhinoCommOut rhino)
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
                var config = await _parser.ParseConfigAsync(configPath);
                _rhino.ShowMessage($"Config parsed from {configPath}");
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
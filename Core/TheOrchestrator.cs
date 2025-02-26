using System;
using System.Threading;
using System.Threading.Tasks;
using Config.Interfaces;
using Interfaces;
using Config;
using Commons;
using Commons.Params;

namespace Core
{
    public class TheOrchestrator : ITheOrchestrator
    {
        private readonly IConfigSelUI _selector;
        private readonly IConfigParser _parser;
        private readonly IRhinoCommOut _rhinoCommOut;

        public TheOrchestrator(
            IConfigSelUI selector,
            IConfigParser parser,
            IRhinoCommOut rhinoCommOut)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
        }

        public async Task<bool> RunBatchAsync(string? configPath, CancellationToken ct)
        {
            try
            {
                _rhinoCommOut.ShowMessage("Starting BatchProcessor...");
                configPath ??= _selector.SelectConfigFile();
                _rhinoCommOut.ShowMessage($"Config file selected: {configPath}");
                if (string.IsNullOrEmpty(configPath) || ct.IsCancellationRequested)
                {
                    _rhinoCommOut.ShowError("Configuration selection canceled.");
                    return false;
                }

                _rhinoCommOut.ShowMessage("Parsing config file...");
                var configResult = await _parser.ParseConfigAsync(configPath);
                _rhinoCommOut.ShowMessage("Config parsing completed.");

                Commons.Logging.ConfigValLog.LogValidationResults(configResult, _rhinoCommOut);

                // Debug output for Commons.ConfigParams
                _rhinoCommOut.ShowMessage("Commons.ConfigParams Contents:");
                _rhinoCommOut.ShowMessage($"BatchDir FileDir: {BatchDir.Instance.FileDir}");
                _rhinoCommOut.ShowMessage($"BatchDir OutputDir: {BatchDir.Instance.OutputDir}");
                _rhinoCommOut.ShowMessage($"ScriptPath FullPath: {ScriptPath.Instance.FullPath}");
                _rhinoCommOut.ShowMessage($"TimeOutMin Minutes: {TimeOutMin.Instance.Minutes}");
                _rhinoCommOut.ShowMessage($"Reprocess Mode: {Reprocess.Instance.Mode}");
                _rhinoCommOut.ShowMessage($"Reprocess ReferenceLog: {Reprocess.Instance.ReferenceLog}");

                _rhinoCommOut.ShowMessage("PIDListLog Contents:");
                foreach (var pidStatus in PIDListLog.Instance.GetPids())
                {
                    _rhinoCommOut.ShowMessage(pidStatus.ToString());
                }

                _rhinoCommOut.ShowMessage("PIDList Contents:");
                foreach (var id in PIDList.Instance.GetUniqueIds())
                {
                    _rhinoCommOut.ShowMessage(id);
                }

                if (!configResult.IsValid)
                    return false;

                _rhinoCommOut.ShowMessage($"Config parsed and validated from {configPath}");
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
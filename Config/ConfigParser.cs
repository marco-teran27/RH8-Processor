using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Config.Models;
using Config.Interfaces;
using Config.Validation;
using Interfaces;

namespace Config
{
    public class ConfigParser : IConfigParser
    {
        private readonly ICommonsDataService _commonsDataService;
        private readonly IRhinoCommOut _rhinoCommOut; // Added for logging

        public ConfigParser(ICommonsDataService commonsDataService, IRhinoCommOut rhinoCommOut)
        {
            _commonsDataService = commonsDataService ?? throw new ArgumentNullException(nameof(commonsDataService));
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
        }

        public async Task<(IConfigDataResults Data, IConfigValResults Val)> ParseConfigAsync(string configPath)
        {
            _rhinoCommOut.ShowMessage($"DEBUG: Starting ParseConfigAsync with path {configPath} at {DateTime.Now}");
            try
            {
                if (string.IsNullOrEmpty(configPath))
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: Config path empty at {DateTime.Now}");
                    return (null, new ConfigValResults(null, configPath, new List<(string, bool, IReadOnlyList<string>)> { ("ConfigPath", false, new List<string> { "Config file path cannot be empty" }) }));
                }

                if (!File.Exists(configPath))
                {
                    _rhinoCommOut.ShowMessage($"DEBUG: Config file not found at {configPath} at {DateTime.Now}");
                    return (null, new ConfigValResults(null, configPath, new List<(string, bool, IReadOnlyList<string>)> { ("ConfigPath", false, new List<string> { $"Config file does not exist: {configPath}" }) }));
                }

                _rhinoCommOut.ShowMessage($"DEBUG: Reading config file at {DateTime.Now}");
                string jsonString = await File.ReadAllTextAsync(configPath);
                _rhinoCommOut.ShowMessage($"DEBUG: Deserializing config at {DateTime.Now}");
                var config = JsonSerializer.Deserialize<ConfigDataResults>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException("Failed to deserialize config file");

                _rhinoCommOut.ShowMessage($"DEBUG: Config deserialized, validating at {DateTime.Now}");
                var valResults = new ConfigValResults(config, configPath);
                _rhinoCommOut.ShowMessage($"DEBUG: Updating commons data at {DateTime.Now}");
                _commonsDataService.UpdateFromConfig(config, valResults);
                _rhinoCommOut.ShowMessage($"DEBUG: ParseConfigAsync completed at {DateTime.Now}");
                return (config, valResults);
            }
            catch (Exception ex)
            {
                _rhinoCommOut.ShowMessage($"DEBUG: ParseConfigAsync failed at {DateTime.Now}: {ex.Message}");
                return (null, new ConfigValResults(null, configPath, new List<(string, bool, IReadOnlyList<string>)> { ("Parsing", false, new List<string> { ex.Message }) }));
            }
        }
    }
}
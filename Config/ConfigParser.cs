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
        private readonly IRhinoCommOut _rhinoCommOut;

        public ConfigParser(ICommonsDataService commonsDataService, IRhinoCommOut rhinoCommOut)
        {
            _commonsDataService = commonsDataService ?? throw new ArgumentNullException(nameof(commonsDataService));
            _rhinoCommOut = rhinoCommOut ?? throw new ArgumentNullException(nameof(rhinoCommOut));
        }

        public async Task<(IConfigDataResults Data, IConfigValResults Val)> ParseConfigAsync(string configPath)
        {
            try
            {
                if (string.IsNullOrEmpty(configPath))
                {
                    return (null, new ConfigValResults(null, configPath, new List<(string, bool, IReadOnlyList<string>)> { ("ConfigPath", false, new List<string> { "Config file path cannot be empty" }) }));
                }

                if (!File.Exists(configPath))
                {
                    return (null, new ConfigValResults(null, configPath, new List<(string, bool, IReadOnlyList<string>)> { ("ConfigPath", false, new List<string> { $"Config file does not exist: {configPath}" }) }));
                }

                string jsonString = await File.ReadAllTextAsync(configPath);
                var config = JsonSerializer.Deserialize<ConfigDataResults>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException("Failed to deserialize config file");

                var valResults = new ConfigValResults(config, configPath);
                _commonsDataService.UpdateFromConfig(config, valResults);
                return (config, valResults);
            }
            catch (Exception ex)
            {
                return (null, new ConfigValResults(null, configPath, new List<(string, bool, IReadOnlyList<string>)> { ("Parsing", false, new List<string> { ex.Message }) }));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Config.Models;
using Config.Validation;
using Config.Interfaces;
using Commons;

namespace Config.ConfigJSON
{
    public class ConfigParser : IConfigParser
    {
        public async Task<ConfigValidationResults> ParseConfigAsync(string configPath)
        {
            try
            {
                // Load entire file into memory (sufficient for now)
                var jsonString = await File.ReadAllTextAsync(configPath);
                var config = JsonSerializer.Deserialize<ConfigStructure>(jsonString);

                if (config == null)
                    return new ConfigValidationResults(false, new[] { "Failed to parse configuration." });

                config.ProjectName.ActualConfigFileName = Path.GetFileName(configPath);

                var validators = new List<IValidator>
                {
                    new ProjectNameValidator(),
                    new DirectoryValidator(),
                    new PIDValidator(),
                    new ReprocessValidator(),
                    new RhinoFileNameValidator(),
                    new ScriptSettingsValidator(),
                    new TimeOutValidator()
                };
                var aggregator = new ConfigValidator(validators);
                return aggregator.ValidateConfig(config, configPath);
            }
            catch (JsonException ex)
            {
                return new ConfigValidationResults(false, new[] { $"JSON parsing error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return new ConfigValidationResults(false, new[] { $"Unexpected error: {ex.Message}" });
            }
        }
    }
}
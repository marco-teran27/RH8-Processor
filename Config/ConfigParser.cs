using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Commons;
using Config.Interfaces;
using Config.Models;
using Config.Validation;

namespace Config
{
    public class ConfigParser : IConfigParser
    {
        private readonly ConfigValidator _validator;

        public ConfigParser(ConfigValidator validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<ConfigValidationResults> ParseConfigAsync(string configPath)
        {
            if (string.IsNullOrEmpty(configPath))
            {
                /// Updated: Add default ConfigStructure when configPath is empty
                return new ConfigValidationResults(false, new List<string> { "Config file path cannot be empty" }, new List<ValidatorResult>(), new ConfigStructure());
            }

            if (!File.Exists(configPath))
            {
                /// Updated: Add default ConfigStructure when file doesn’t exist
                return new ConfigValidationResults(false, new List<string> { $"Config file does not exist: {configPath}" }, new List<ValidatorResult>(), new ConfigStructure());
            }

            try
            {
                var jsonString = await File.ReadAllTextAsync(configPath);
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    /// Updated: Add default ConfigStructure when JSON is empty
                    return new ConfigValidationResults(false, new List<string> { "Config file cannot be empty" }, new List<ValidatorResult>(), new ConfigStructure());
                }

                var config = JsonSerializer.Deserialize<ConfigStructure>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (config == null)
                {
                    /// Updated: Add default ConfigStructure when deserialization fails
                    return new ConfigValidationResults(false, new List<string> { "Failed to deserialize config file" }, new List<ValidatorResult>(), new ConfigStructure());
                }

                return _validator.ValidateConfig(config, configPath);
            }
            catch (JsonException ex)
            {
                return new ConfigValidationResults(false, new List<string> { $"Invalid JSON format: {ex.Message}" }, new List<ValidatorResult>(), new ConfigStructure());
            }
            catch (Exception ex)
            {
                return new ConfigValidationResults(false, new List<string> { $"Error parsing config: {ex.Message}" }, new List<ValidatorResult>(), new ConfigStructure());
            }
        }
    }
}
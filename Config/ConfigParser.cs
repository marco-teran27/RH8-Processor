using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Config.Models;
using Config.Validation;
using Config.Interfaces;
using Commons;

namespace Config
{
    public class ConfigParser : IConfigParser
    {
        public Task<ConfigValidationResults> ParseConfigAsync(string configPath)
        {
            try
            {
                Console.WriteLine($"Starting ParseConfigAsync with path: {configPath}");
                Console.WriteLine("Checking file existence...");
                if (!File.Exists(configPath))
                    return Task.FromResult(new ConfigValidationResults(false, new[] { $"File does not exist: {configPath}" }));

                Console.WriteLine("Reading config file synchronously...");
                var jsonString = File.ReadAllText(configPath);
                Console.WriteLine("Config file read, deserializing...");
                var config = JsonSerializer.Deserialize<ConfigStructure>(jsonString);
                Console.WriteLine("Deserialization completed.");

                if (config == null)
                    return Task.FromResult(new ConfigValidationResults(false, new[] { "Failed to parse configuration." }));

                config.ProjectName.ActualConfigFileName = Path.GetFileName(configPath);
                Console.WriteLine("ActualConfigFileName set, initializing validators...");

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
                Console.WriteLine("Validators initialized, validating config...");
                var result = aggregator.ValidateConfig(config, configPath);
                Console.WriteLine("Validation completed.");
                return Task.FromResult(result);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON parsing error: {ex.Message}");
                return Task.FromResult(new ConfigValidationResults(false, new[] { $"JSON parsing error: {ex.Message}" }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return Task.FromResult(new ConfigValidationResults(false, new[] { $"Unexpected error: {ex.Message}" }));
            }
        }
    }
}
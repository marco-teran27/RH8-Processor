using System;
using System.Collections.Generic;
using Config.Models;
using Commons;
using Config.Interfaces;

namespace Config.Validation
{
    public class ConfigValidator
    {
        private readonly IEnumerable<IValidator> _validators;

        public ConfigValidator(IEnumerable<IValidator> validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public ConfigValidationResults ValidateConfig(ConfigStructure config, string configPath)
        {
            var errorMessages = new List<string>();

            foreach (var validator in _validators)
            {
                var (isValid, errorMessage) = validator.ValidateConfig(
                    config.ProjectName,
                    config.DirectorySettings,
                    config.PIDSettings,
                    config.RhinoFileNameSettings,
                    config.ScriptSettings,
                    config.ReprocessSettings,
                    config.TimeoutSettings);

                if (!isValid && !string.IsNullOrEmpty(errorMessage))
                    errorMessages.Add(errorMessage);
            }

            return new ConfigValidationResults(
                errorMessages.Count == 0,
                errorMessages,
                errorMessages.Count == 0 ? config : null,
                configPath);
        }
    }
}
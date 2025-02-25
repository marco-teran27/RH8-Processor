using System;
using System.Collections.Generic;
using Config.Models;
using Commons;
using Config.Interfaces;
using System.ComponentModel.DataAnnotations;

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
            var validatorResults = new List<ValidatorResult>();

            foreach (var validator in _validators)
            {
                var (isValid, errorMessage) = validator.ValidateConfig(
                    config.ProjectName,
                    config.Directories,
                    config.PidSettings,
                    config.RhinoFileNameSettings,
                    config.ScriptSettings,
                    config.ReprocessSettings,
                    config.TimeoutSettings);

                var validatorName = validator.GetType().Name.Replace("Validator", "");
                validatorResults.Add(new ValidatorResult(validatorName, isValid, isValid ? "Passed" : errorMessage));

                if (!isValid && !string.IsNullOrEmpty(errorMessage))
                    errorMessages.Add(errorMessage);
            }

            return new ConfigValidationResults(
                errorMessages.Count == 0,
                errorMessages,
                validatorResults);
        }
    }
}
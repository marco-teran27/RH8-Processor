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
            var validatorResults = new List<ValidatorResult>();

            foreach (var validator in _validators)
            {
                var (isValid, messages) = validator.ValidateConfig(
                    config.ProjectName,
                    config.Directories,
                    config.PidSettings,
                    config.RhinoFileNameSettings,
                    config.ScriptSettings,
                    config.ReprocessSettings,
                    config.TimeoutSettings);

                var validatorName = validator.GetType().Name.Replace("Validator", "");
                validatorResults.Add(new ValidatorResult(validatorName, isValid, messages));

                if (!isValid && messages != null)
                    errorMessages.AddRange(messages);
            }

            /// Updated: Pass config as IConfigStructure
            return new ConfigValidationResults(
                errorMessages.Count == 0,
                errorMessages,
                validatorResults,
                config);
        }
    }
}
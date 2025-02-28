using System.Collections.Generic;
using System.IO;
using Config.Interfaces;
using Commons.Interfaces;
using Commons.Params;
using Interfaces;

namespace FileDir
{
    public class RhinoFileDirValidator : IValidator
    {
        private readonly IRhinoCommOut _rhinoCommOut;

        public RhinoFileDirValidator(IRhinoCommOut rhinoCommOut)
        {
            _rhinoCommOut = rhinoCommOut;
        }

        public string ValidatorName => "RhinoFileDir";

        public IValidationResult Validate() // Updated: Return IValidationResult
        {
            var messages = new List<string>();
            bool isValid = true;

            if (string.IsNullOrEmpty(BatchDir.Instance.FileDir) || !Directory.Exists(BatchDir.Instance.FileDir))
            {
                messages.Add("file_dir: missing or invalid");
                isValid = false;
            }
            else
            {
                messages.Add("file_dir: found");
            }

            return new ValidationResult(ValidatorName, messages, isValid);
        }

        /// Updated: Implement IValidationResult internally
        private class ValidationResult : IValidationResult
        {
            public string ValidatorName { get; }
            public bool IsValid { get; }
            public IReadOnlyList<string> Messages { get; }

            public ValidationResult(string validatorName, List<string> messages, bool isValid)
            {
                ValidatorName = validatorName;
                Messages = messages.AsReadOnly();
                IsValid = isValid;
            }
        }
    }
}
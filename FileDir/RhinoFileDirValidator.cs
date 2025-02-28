using System.Collections.Generic;
using System.IO;
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

        public ValidationResult Validate()
        {
            var messages = new List<string>();
            bool isValid = true;

            /// Validate FileDir exists
            if (string.IsNullOrEmpty(BatchDir.Instance.FileDir) || !Directory.Exists(BatchDir.Instance.FileDir))
            {
                messages.Add("file_dir: missing or invalid");
                isValid = false;
            }
            else
            {
                messages.Add("file_dir: found");
            }

            /// Placeholder: Add more validation (e.g., file count, PID match)
            return new ValidationResult(ValidatorName, messages, isValid);
        }
    }

    public class ValidationResult
    {
        public string ValidatorName { get; }
        public List<string> Messages { get; }
        public bool IsValid { get; }

        public ValidationResult(string validatorName, List<string> messages, bool isValid)
        {
            ValidatorName = validatorName;
            Messages = messages;
            IsValid = isValid;
        }
    }
}
using System.Collections.Generic;

/*
Summary: Defines the ValidationResult class used to encapsulate the result of a validation operation.
         Contains an IsValid flag and a read-only list of error messages.
*/

namespace Commons
{
    /// <summary>
    /// Represents the result of a validation operation.
    /// </summary>
    public class ConfigValidationResults
    {
        /// <summary>
        /// Indicates whether the validation succeeded.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Read-only collection of error messages.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the ValidationResult class.
        /// </summary>
        /// <param name="isValid">True if validation succeeded; otherwise, false.</param>
        /// <param name="errors">Collection of error messages.</param>
        public ConfigValidationResults(bool isValid, IReadOnlyList<string> errors)
        {
            IsValid = isValid;
            Errors = errors;
        }

        /// <summary>
        /// Gets a successful ValidationResult (true with no errors).
        /// </summary>
        public static ConfigValidationResults Success => new ConfigValidationResults(true, new List<string>());
    }
}
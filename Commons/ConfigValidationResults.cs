using System.Collections.Generic;
using Commons.Interfaces;

namespace Commons
{
    public record ValidatorResult(string ValidatorName, bool IsValid, IReadOnlyList<string> Messages);

    public record ConfigValidationResults(
        bool IsValid,
        IReadOnlyList<string> Errors,
        IReadOnlyList<ValidatorResult> ValidatorResults,
        /// Updated: Use interface to avoid Config dependency
        IConfigStructure ConfigStructure);
}
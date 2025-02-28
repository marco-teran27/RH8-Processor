using System.Collections.Generic;

namespace Commons.Interfaces
{
    public interface IValidationResult
    {
        string ValidatorName { get; }
        bool IsValid { get; }
        IReadOnlyList<string> Messages { get; }
    }
}
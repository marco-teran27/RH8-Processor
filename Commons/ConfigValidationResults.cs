namespace Commons
{
    /// <summary>
    /// Represents the result of an individual validator's operation.
    /// </summary>
    public record ValidatorResult(string ValidatorName, bool IsValid, string Message);

    /// <summary>
    /// Represents the result of a validation operation, including overall status and individual validator results.
    /// </summary>
    public record ConfigValidationResults(
        bool IsValid,
        IReadOnlyList<string> Errors,
        IReadOnlyList<ValidatorResult> ValidatorResults);
}
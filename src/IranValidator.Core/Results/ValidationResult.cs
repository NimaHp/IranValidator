namespace IranValidator.Core.Results;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public readonly struct ValidationResult
{
    /// <summary>Gets whether the validation was successful.</summary>
    public bool Success { get; }

    /// <summary>Gets the normalized value, or null if validation failed.</summary>
    public string? NormalizedValue { get; }

    /// <summary>Gets the error code, or <see cref="ValidationErrorCode.None"/> if successful.</summary>
    public ValidationErrorCode ErrorCode { get; }

    private ValidationResult(bool success, string? normalizedValue, ValidationErrorCode errorCode)
    {
        Success = success;
        NormalizedValue = normalizedValue;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static ValidationResult Ok(string? normalizedValue = null)
        => new(true, normalizedValue, ValidationErrorCode.None);

    /// <summary>
    /// Creates a failed validation result with the specified error code.
    /// </summary>
    public static ValidationResult Error(ValidationErrorCode errorCode)
        => new(false, null, errorCode);
}

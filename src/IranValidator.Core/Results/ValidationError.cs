namespace IranValidator.Core.Results;

/// <summary>
/// Represents the result of a validation operation with detailed error information.
/// </summary>
public sealed class ValidationError
{
    /// <summary>Gets the error code.</summary>
    public ValidationErrorCode Code { get; }

    /// <summary>Gets the property name that failed validation, if applicable.</summary>
    public string? PropertyName { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationError"/>.
    /// </summary>
    public ValidationError(ValidationErrorCode code, string? propertyName = null)
    {
        Code = code;
        PropertyName = propertyName;
    }
}

using IranValidator.Core.Results;

namespace IranValidator.Core;

/// <summary>
/// Defines a validator for string values with Span-based support.
/// </summary>
public interface IStringValidator : IValidator<string>
{
    /// <summary>
    /// Validates the specified string value.
    /// </summary>
    new ValidationResult Validate(string value);

    /// <summary>
    /// Validates the specified character span.
    /// </summary>
    ValidationResult Validate(ReadOnlySpan<char> value);
}

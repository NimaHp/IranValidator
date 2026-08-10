using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian postal codes.
/// Pattern: 10-digit numeric code.
/// </summary>
public sealed class PostalCodeValidator : IStringValidator
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static PostalCodeValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    private PostalCodeValidator() { }

    /// <inheritdoc/>
    public ValidationResult Validate(string value)
    {
        if (value is null || value.Length == 0)
            return ValidationResult.Error(ValidationErrorCode.ValueEmpty);

        return ValidateCore(value.AsSpan(), value);
    }

    /// <inheritdoc/>
    public ValidationResult Validate(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return ValidationResult.Error(ValidationErrorCode.ValueEmpty);

        return ValidateCore(value, null);
    }

    private static ValidationResult ValidateCore(ReadOnlySpan<char> value, string? original)
    {

        // Hard input-size bound BEFORE normalization — oversized payloads fail
        // fast and never scale scratch buffers with input size. Legitimate
        // formatted values (digits + spaces/dashes/marks) stay far below this.
        if (value.Length > ValidationConstants.MaxInputLength)
            return ValidationResult.Error(ValidationErrorCode.ValueTooLarge);

        // Normalize input
        string normalized = Normalizer.Normalize(value, original);

        // Whitespace-only input normalizes to empty — report it as an empty value.
        if (normalized.Length == 0)
            return ValidationResult.Error(ValidationErrorCode.ValueEmpty);
        ReadOnlySpan<char> postalCode = normalized.AsSpan();

        // Check length
        if (postalCode.Length != ValidationConstants.PostalCodeLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

        // All characters must be digits
        for (int i = 0; i < postalCode.Length; i++)
        {
            if (!postalCode[i].IsAsciiDigit())
                return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);
        }

        // First digit must be non-zero
        if (postalCode[0] == '0')
            return ValidationResult.Error(ValidationErrorCode.InvalidFormat);

        return ValidationResult.Ok(normalized);
    }
}

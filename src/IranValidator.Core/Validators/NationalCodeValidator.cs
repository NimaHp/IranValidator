using IranValidator.Core.Algorithms;
using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian National Code (کد ملی).
/// Algorithm: Weighted-sum checksum (10 digits).
/// </summary>
public sealed class NationalCodeValidator : IStringValidator
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static NationalCodeValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    private NationalCodeValidator() { }

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
        ReadOnlySpan<char> code = normalized.AsSpan();

        // Check length
        if (code.Length != ValidationConstants.NationalCodeLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

        // Validate characters
        for (int i = 0; i < code.Length; i++)
        {
            if (!code[i].IsAsciiDigit() && !code[i].IsPersianOrArabicDigit())
                return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);
        }

        // Algorithm validation
        if (!NationalCodeAlgorithm.Validate(code))
            return ValidationResult.Error(ValidationErrorCode.InvalidChecksum);

        return ValidationResult.Ok(normalized);
    }
}

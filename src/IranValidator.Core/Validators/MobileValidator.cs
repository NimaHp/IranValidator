using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian mobile numbers.
/// Pattern: 09xx + 11 digits total.
/// </summary>
public sealed class MobileValidator : IStringValidator
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static MobileValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    private MobileValidator() { }

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

        // Normalize input
        string normalized = Normalizer.Normalize(value, original);

        // Whitespace-only input normalizes to empty — report it as an empty value.
        if (normalized.Length == 0)
            return ValidationResult.Error(ValidationErrorCode.ValueEmpty);
        ReadOnlySpan<char> mobile = normalized.AsSpan();

        // Must be exactly 11 digits
        if (mobile.Length != ValidationConstants.MobileLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

        // Must start with 09
        if (mobile[0] != '0' || mobile[1] != '9')
            return ValidationResult.Error(ValidationErrorCode.InvalidFormat);

        // Validate all characters are digits
        for (int i = 0; i < mobile.Length; i++)
        {
            if (!mobile[i].IsAsciiDigit())
                return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);
        }

        // Validate 4-digit operator prefix (09XX) against the list of assigned prefixes.
        int prefix = (mobile[0] - '0') * 1000 + (mobile[1] - '0') * 100 + (mobile[2] - '0') * 10 + (mobile[3] - '0');
        if (!MobilePrefixes.Contains(prefix))
            return ValidationResult.Error(ValidationErrorCode.InvalidFormat);

        return ValidationResult.Ok(normalized);
    }
}

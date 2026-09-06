using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian Passport Number (شماره گذرنامه).
/// Current format (1405): 1 letter + 8 digits (9 characters).
/// Valid letters: A, B, F, H, P, U, V, W, X, Y.
/// Legacy 8-digit numeric format is deprecated since 1405 and disabled by default;
/// enable <see cref="AllowLegacy8Digit"/> only for archive compatibility.
/// No checksum algorithm.
/// </summary>
public sealed class PassportValidator : IStringValidator
{
    /// <summary>
    /// When true, allows legacy 8-digit numeric passports for archive/backward compatibility.
    /// Deprecated since 1405 — no valid 8-digit passports are in circulation.
    /// Default is <c>false</c> (strict 9-character validation).
    /// </summary>
    // For archive compatibility only — deprecated.
#pragma warning disable CA1805 // False positive: explicit ' = false' documents the default.
    public static bool AllowLegacy8Digit { get; set; } = false;
#pragma warning restore CA1805

    /// <summary>
    /// Valid passport series letters (first character of new-format passports).
    /// </summary>
    /// <remarks>
    /// Iranian passport numbers use these letters as prefix:
    /// A, B, F, H (diplomatic/service), P (ordinary), U, V, W, X, Y.
    /// </remarks>
    private static readonly System.Collections.Generic.HashSet<char> ValidLetters = new(
        ['A', 'B', 'F', 'H', 'P', 'U', 'V', 'W', 'X', 'Y']);

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static PassportValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    private PassportValidator() { }

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
        ReadOnlySpan<char> passport = normalized.AsSpan();

        // Check length
        if (passport.Length < ValidationConstants.PassportMinLength ||
            passport.Length > ValidationConstants.PassportMaxLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

        // Validate format
        if (passport.Length == 9)
        {
            // Format: 1 letter + 8 digits
            char first = char.ToUpperInvariant(passport[0]);

            if (!ValidLetters.Contains(first))
                return ValidationResult.Error(ValidationErrorCode.InvalidFormat);

            // Check remaining 8 chars are digits
            for (int i = 1; i < passport.Length; i++)
            {
                if (!passport[i].IsAsciiDigit())
                    return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);
            }

            // Return normalized with uppercase letter; reuse the already-uppercased
            // 'first' and avoid allocating when the letter was already uppercase.
            string result = first == passport[0] ? normalized : first + normalized[1..];
            return ValidationResult.Ok(result);
        }
        else // Length == 8 — legacy format (deprecated since 1405)
        {
            // Legacy 8-digit passports are no longer in circulation as of 1405.
            // Return InvalidFormat when legacy support is disabled (strict mode).
            if (!AllowLegacy8Digit)
                return ValidationResult.Error(ValidationErrorCode.InvalidFormat);

            // AllowLegacy8Digit == true: preserve legacy behavior (8 ASCII digits) for archive compatibility.
            for (int i = 0; i < passport.Length; i++)
            {
                if (!passport[i].IsAsciiDigit())
                    return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);
            }

            return ValidationResult.Ok(normalized);
        }
    }
}

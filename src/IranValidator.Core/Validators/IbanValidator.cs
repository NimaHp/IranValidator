using IranValidator.Core.Algorithms;
using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian IBAN (شبا) numbers using ISO 13616 MOD-97 algorithm.
/// </summary>
public sealed class IbanValidator : IStringValidator
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static IbanValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    private IbanValidator() { }

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

        // Normalize (remove spaces, dashes, direction marks, normalize digits)
        string normalized = Normalizer.Normalize(value, original);

        // Whitespace-only input normalizes to empty — report it as an empty value.
        if (normalized.Length == 0)
            return ValidationResult.Error(ValidationErrorCode.ValueEmpty);

        // Normalizer converts digits only, IBAN letters stay
        ReadOnlySpan<char> iban = normalized.AsSpan();

        // Must be exactly 26 characters
        if (iban.Length != ValidationConstants.IbanLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

        // Must start with IR (case-insensitive: lowercase input is normalized
        // to uppercase so NormalizedValue is canonical)
        bool upperPrefix = iban[0] == 'I' && iban[1] == 'R';
        bool lowerPrefix = iban[0] == 'i' && iban[1] == 'r';
        if (!upperPrefix && !lowerPrefix)
            return ValidationResult.Error(ValidationErrorCode.InvalidFormat);

        if (lowerPrefix)
        {
            // Uppercase BOTH prefix letters so NormalizedValue is canonical "IR…".
            normalized = $"{char.ToUpperInvariant(iban[0])}{char.ToUpperInvariant(iban[1])}{normalized[2..]}";
            iban = normalized.AsSpan();
        }

        // Delegate to IBAN algorithm for MOD-97 checksum
        if (!IbanAlgorithm.Validate(iban))
            return ValidationResult.Error(ValidationErrorCode.InvalidChecksum);

        // Iranian-specific: positions 4-6 (0-based) hold the 3-digit bank code.
        // A checksum-valid IBAN with an unknown bank code (e.g. 999) must be
        // rejected — MOD-97 alone cannot catch it.
        if (!iban[4].IsAsciiDigit() || !iban[5].IsAsciiDigit() || !iban[6].IsAsciiDigit())
            return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);

        int bankCode = (iban[4] - '0') * 100 + (iban[5] - '0') * 10 + (iban[6] - '0');
        if (!IranianShebaBankCodes.Contains(bankCode))
            return ValidationResult.Error(ValidationErrorCode.InvalidBankCode);

        return ValidationResult.Ok(normalized);
    }
}

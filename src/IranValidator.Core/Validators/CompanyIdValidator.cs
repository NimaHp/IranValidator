using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian Company ID (شناسه ملی شرکت).
/// 11-digit identifier with weighted checksum modulo 11.
/// </summary>
public sealed class CompanyIdValidator : IStringValidator
{
    private static readonly int[] Weights = { 29, 27, 23, 19, 17, 13, 7, 5, 3, 2 };

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static CompanyIdValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    private CompanyIdValidator() { }

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

        // Must be exactly 11 digits
        if (code.Length != ValidationConstants.CompanyIdLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

        // Validate all characters are digits
        for (int i = 0; i < code.Length; i++)
        {
            if (!code[i].IsAsciiDigit())
                return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);
        }

        // All same digits is invalid
        bool allSame = true;
        for (int i = 1; i < code.Length; i++)
        {
            if (code[i] != code[0])
            {
                allSame = false;
                break;
            }
        }
        if (allSame)
            return ValidationResult.Error(ValidationErrorCode.InvalidFormat);

        // Calculate weighted sum for first 10 digits
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            int digit = code[i].DigitToInt();
            // digit is guaranteed >= 0 because we validated all digits above
            sum += digit * Weights[i];
        }

        int remainder = sum % 11;
        int checkDigit = code[10].DigitToInt();
        int expectedCheckDigit = remainder < 2 ? remainder : 11 - remainder;

        if (checkDigit != expectedCheckDigit)
            return ValidationResult.Error(ValidationErrorCode.InvalidChecksum);

        return ValidationResult.Ok(normalized);
    }
}

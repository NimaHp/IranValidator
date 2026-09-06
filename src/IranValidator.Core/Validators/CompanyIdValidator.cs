using IranValidator.Core.Algorithms;
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

        // All same digits and checksum are handled by CompanyIdAlgorithm;
        // keep an early all-same fast-path for the InvalidFormat error code
        // distinction, then delegate checksum to the algorithm.

        if (!CompanyIdAlgorithm.Validate(code))
        {
            // Distinguish allSame vs checksum: re-check allSame to preserve
            // the original InvalidFormat vs InvalidChecksum error semantics.
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

            return ValidationResult.Error(ValidationErrorCode.InvalidChecksum);
        }

        return ValidationResult.Ok(normalized);
    }
}

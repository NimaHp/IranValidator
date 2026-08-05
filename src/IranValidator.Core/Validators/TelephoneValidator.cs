using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian landline telephone numbers.
/// Pattern: 0 + 2-digit province area code + 8-digit local number = 11 digits total.
/// The area code must be one of the 31 assigned provincial codes
/// (e.g. 021 Tehran, 011 Mazandaran, 051 Mashhad) — see <see cref="ProvinceAreaCodes"/>.
/// Since the nationwide area-code unification (طرح هم‌کدسازی, 2014) county-level
/// codes no longer exist; each province has a single 2-digit code.
/// The 8-digit local number must start with 2-9: 0 is the national trunk prefix
/// (within the province the number is dialed without it), and 1 is reserved for
/// 3-digit service numbers (110, 115, 125, 123, ...).
/// </summary>
public sealed class TelephoneValidator : IStringValidator
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static TelephoneValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    private TelephoneValidator() { }

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
        ReadOnlySpan<char> tel = normalized.AsSpan();

        // Must be exactly 11 digits
        if (tel.Length != ValidationConstants.TelephoneLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

        // Must start with the trunk prefix 0
        if (tel[0] != '0')
            return ValidationResult.Error(ValidationErrorCode.InvalidFormat);

        // Validate all characters are digits
        for (int i = 0; i < tel.Length; i++)
        {
            if (!tel[i].IsAsciiDigit())
                return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);
        }

        // Area code = digits[1..3]; must be an assigned province code.
        // Mobile numbers (09x) are rejected here: 9x is not a landline area code.
        ushort areaCode = (ushort)((tel[1] - '0') * 10 + (tel[2] - '0'));
        if (!ProvinceAreaCodes.Contains(areaCode))
            return ValidationResult.Error(ValidationErrorCode.InvalidAreaCode);

        // Local subscriber number (digits 4-11) must start with 2-9:
        // - 0 is the national trunk prefix — it cannot lead a local number,
        //   because within the province the number is dialed without it.
        // - 1 is reserved for 3-digit service numbers (110, 115, 125, 123, ...).
        if (tel[3] < ValidationConstants.TelephoneLocalFirstDigitMin ||
            tel[3] > ValidationConstants.TelephoneLocalFirstDigitMax)
            return ValidationResult.Error(ValidationErrorCode.InvalidFormat);

        return ValidationResult.Ok(normalized);
    }
}

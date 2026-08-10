using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian Vehicle Plate (پلاک خودرو), covering both car and
/// motorcycle plates.
/// Car format: 2 digits + 1 Persian letter + 3 digits + 2 digits (province
/// code) = 8 chars. Example: "12ب34567" → row 12, letter ب, sequence 345,
/// province 67. No checksum algorithm.
/// Motorcycle format: 8 digits (3-digit province code + 5-digit serial),
/// no letter. Example: "12345678" → province 123, serial 45678.
/// </summary>
/// <remarks>
/// A plate whose 8 characters are all digits is treated as a motorcycle plate
/// and validated against <see cref="MotorcycleProvinceCodes"/> (first three
/// digits); any other 8-char plate is treated as a car plate.
/// Car: the letter at position 3 must be one of the 24 official Persian
/// issuance letters (ا ب پ ت ث ج د ز ژ س ش ص ط ع ف ق ک گ ل م ن و ه ی) or the
/// two Latin service letters D (diplomatic) and S (embassy). Latin
/// transliterations of Persian letters (B, J, V, ...) are NOT accepted; Arabic
/// letter variants (ي، ك) are normalized to their Persian equivalents (ی، ک)
/// first. Car digits are 1–9; only the final digit (second digit of the
/// province code) may be 0 — e.g. Tehran 10/20/.../70 — matching how real
/// plates are issued. The final two digits are the issuing province code,
/// validated against <see cref="IranianProvinceCodes"/>.
/// Motorcycle plates never print 0: every digit is issued in the range 1–9.
/// The word «ایران» printed on plates (e.g. «۱۲ ب ۳۴۵ ایران ۶۷») is accepted
/// and stripped during normalization.
/// </remarks>
public sealed class VehiclePlateValidator : IStringValidator
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static VehiclePlateValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    // Official issuance letters: 24 Persian + D (diplomatic) + S (embassy).
    // Source: Wikipedia «پلاک وسایل نقلیه در ایران»
    private static readonly System.Collections.Generic.HashSet<char> ValidLetters =
    [
        'ا', 'ب', 'پ', 'ت', 'ث', 'ج', 'د', 'ز', 'ژ', 'س', 'ش', 'ص', 'ط', 'ع', 'ف',
        'ق', 'ک', 'گ', 'ل', 'م', 'ن', 'و', 'ه', 'ی',
        'D', 'S',
    ];

    private VehiclePlateValidator() { }

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
        ReadOnlySpan<char> plate = normalized.AsSpan();

        // Check length: exactly 8 chars (car: 2+1+3+2; motorcycle: 3+5)
        if (plate.Length != ValidationConstants.VehiclePlateLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

        // Motorcycle plates are 8 digits (3-digit province code + 5-digit
        // serial) with no letter; car plates always carry one at index 2, so
        // an all-digit 8-char plate is unambiguously a motorcycle.
        if (IsAllDigits(plate))
            return ValidateMotorcycle(plate, normalized);

        // Format: digit digit letter digit digit digit digit digit
        for (int i = 0; i < plate.Length; i++)
        {
            if (i == 2)
            {
                char c = plate[i];
                if (!ValidLetters.Contains(c))
                    return ValidationResult.Error(ValidationErrorCode.InvalidFormat);
            }
            else
            {
                char c = plate[i];
                if (!c.IsAsciiDigit())
                    return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);

                // Plate digits are 1–9; only the final digit (second digit of
                // the province code) may be 0 — e.g. Tehran 10/20/.../70. This
                // matches persian-tools and real plates, which never print 0
                // in the first two digits or the middle sequence.
                if (c == '0' && i != plate.Length - 1)
                    return ValidationResult.Error(ValidationErrorCode.InvalidFormat);
            }
        }

        // Province code: the last two digits must be an assigned issuance code.
        int provinceCode = (plate[6] - '0') * 10 + (plate[7] - '0');
        if (!IranianProvinceCodes.Contains(provinceCode))
            return ValidationResult.Error(ValidationErrorCode.InvalidProvinceCode);

        return ValidationResult.Ok(normalized);
    }

    private static bool IsAllDigits(ReadOnlySpan<char> value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (!value[i].IsAsciiDigit())
                return false;
        }
        return true;
    }

    private static ValidationResult ValidateMotorcycle(ReadOnlySpan<char> plate, string normalized)
    {
        // Motorcycle plates never print the digit 0 — unlike car plates, where
        // the final digit (province codes like 10/20/30) may be 0. Every one
        // of the 8 digits is issued in the range 1–9.
        for (int i = 0; i < plate.Length; i++)
        {
            if (plate[i] == '0')
                return ValidationResult.Error(ValidationErrorCode.InvalidFormat);
        }

        // Province code: the first three digits must be an assigned code.
        int provinceCode = (plate[0] - '0') * 100 + (plate[1] - '0') * 10 + (plate[2] - '0');
        if (!MotorcycleProvinceCodes.Contains(provinceCode))
            return ValidationResult.Error(ValidationErrorCode.InvalidProvinceCode);

        return ValidationResult.Ok(normalized);
    }
}

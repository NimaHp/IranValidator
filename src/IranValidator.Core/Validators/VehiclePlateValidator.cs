using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian Vehicle Plate (پلاک خودرو).
/// Format: 2 digits + 1 Persian letter + 3 digits + 2 digits (province code) = 8 chars.
/// Example: "12ب34567" → row 12, letter ب, sequence 345, province 67.
/// No checksum algorithm.
/// </summary>
/// <remarks>
/// The letter at position 3 must be one of the 23 official Persian issuance
/// letters (ا ب پ ت ث ج د ژ س ش ص ط ع ف ق ک گ ل م ن و ه ی) or the two Latin
/// service letters D (diplomatic) and S (embassy). Latin transliterations of
/// Persian letters (B, J, V, ...) are NOT accepted. The final two digits are
/// the issuing province code, validated against
/// <see cref="IranianProvinceCodes"/>.
/// </remarks>
public sealed class VehiclePlateValidator : IStringValidator
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static VehiclePlateValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    // Official issuance letters: 23 Persian + D (diplomatic) + S (embassy).
    // Source: Wikipedia «پلاک وسایل نقلیه در ایران»; matches persian-tools (JS)
    // numberplate Category set (which also lists exactly these 25 letters).
    private static readonly System.Collections.Generic.HashSet<char> ValidLetters =
    [
        'ا', 'ب', 'پ', 'ت', 'ث', 'ج', 'د', 'ژ', 'س', 'ش', 'ص', 'ط', 'ع', 'ف',
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

        // Normalize input
        string normalized = Normalizer.Normalize(value, original);

        // Whitespace-only input normalizes to empty — report it as an empty value.
        if (normalized.Length == 0)
            return ValidationResult.Error(ValidationErrorCode.ValueEmpty);
        ReadOnlySpan<char> plate = normalized.AsSpan();

        // Check length: exactly 8 chars (2 digits + 1 letter + 3 digits + 2 digits)
        if (plate.Length != ValidationConstants.VehiclePlateLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

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
                if (!plate[i].IsAsciiDigit())
                    return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);
            }
        }

        // Province code: the last two digits must be an assigned issuance code.
        int provinceCode = (plate[6] - '0') * 10 + (plate[7] - '0');
        if (!IranianProvinceCodes.Contains(provinceCode))
            return ValidationResult.Error(ValidationErrorCode.InvalidProvinceCode);

        return ValidationResult.Ok(normalized);
    }
}

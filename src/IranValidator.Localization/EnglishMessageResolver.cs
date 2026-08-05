using System.Globalization;
using IranValidator.Core.Results;

namespace IranValidator.Localization;

/// <summary>
/// English (invariant) implementation of <see cref="IValidationMessageResolver"/>.
/// Used as the default fallback when no culture-specific resolver is registered.
/// </summary>
public sealed class EnglishMessageResolver : IValidationMessageResolver
{
    /// <inheritdoc />
    public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
    {
        var name = propertyName ?? "Value";

        return errorCode switch
        {
            ValidationErrorCode.ValueEmpty => $"{name} cannot be empty.",
            ValidationErrorCode.InvalidLength => $"{name} has an invalid length.",
            ValidationErrorCode.InvalidChecksum => $"{name} has an invalid checksum.",
            ValidationErrorCode.InvalidFormat => $"{name} has an invalid format.",
            ValidationErrorCode.InvalidCharacters => $"{name} contains invalid characters.",
            ValidationErrorCode.InvalidProvinceCode => $"{name} has an invalid province code.",
            ValidationErrorCode.InvalidBankCode => $"{name} has an invalid bank code.",
            ValidationErrorCode.InvalidAreaCode => $"{name} has an invalid area code.",
            ValidationErrorCode.UnsupportedIssuer => $"{name} is not issued by an Iranian bank.",
            _ => $"{name} is not valid."
        };
    }
}

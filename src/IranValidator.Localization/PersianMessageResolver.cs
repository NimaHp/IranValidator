using System.Globalization;
using IranValidator.Core.Results;

namespace IranValidator.Localization;

/// <summary>
/// Persian (Farsi — fa-IR) implementation of <see cref="IValidationMessageResolver"/>.
/// </summary>
public sealed class PersianMessageResolver : IValidationMessageResolver
{
    /// <inheritdoc />
    public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
    {
        var name = propertyName ?? "مقدار";

        return errorCode switch
        {
            ValidationErrorCode.ValueEmpty => $"{name} نمی‌تواند خالی باشد.",
            ValidationErrorCode.InvalidLength => $"طول {name} نامعتبر است.",
            ValidationErrorCode.InvalidChecksum => $"مجموع ارقام {name} نامعتبر است.",
            ValidationErrorCode.InvalidFormat => $"فرمت {name} نامعتبر است.",
            ValidationErrorCode.InvalidCharacters => $"{name} شامل کاراکترهای نامعتبر است.",
            ValidationErrorCode.InvalidProvinceCode => $"کد استان {name} نامعتبر است.",
            ValidationErrorCode.InvalidBankCode => $"کد بانک {name} نامعتبر است.",
            ValidationErrorCode.InvalidAreaCode => $"پیش‌شماره {name} نامعتبر است.",
            ValidationErrorCode.UnsupportedIssuer => $"{name} متعلق به هیچ بانک ایرانی نیست.",
            _ => $"{name} معتبر نیست."
        };
    }
}

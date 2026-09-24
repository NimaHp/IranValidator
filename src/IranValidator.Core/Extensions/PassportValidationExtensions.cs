using IranValidator.Core.Results;
using IranValidator.Core.Validators;

namespace IranValidator.Core.Extensions;

public static class PassportValidationExtensions
{
    public static bool IsIranPassport(this string value, bool allowLegacy8Digit)
        => (allowLegacy8Digit ? PassportValidator.CreateArchiveValidator() : PassportValidator.Instance).Validate(value).Success;

    public static ValidationResult ValidateIranPassport(this string value, bool allowLegacy8Digit)
        => (allowLegacy8Digit ? PassportValidator.CreateArchiveValidator() : PassportValidator.Instance).Validate(value);
}

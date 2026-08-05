using System.Globalization;
using IranValidator.Core;
using IranValidator.Localization;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Base class for Persian data validation attributes. Null values are valid
/// (DataAnnotations convention); non-string values fail with a type message.
/// Validation messages are resolved through <see cref="IValidationMessageResolver"/>:
/// from the validation context's service provider when available (ASP.NET Core
/// after <c>AddIranValidation</c>), otherwise through the static
/// <see cref="IranDataAnnotationsLocalization"/> registry.
/// </summary>
public abstract class IranValidationAttribute : ValidationAttribute
{
    /// <summary>
    /// Gets the underlying string validator used by this attribute.
    /// </summary>
    protected abstract IStringValidator Validator { get; }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        if (value is not string str)
            return new ValidationResult("The value must be a string.");

        var result = Validator.Validate(str);
        if (result.Success)
            return ValidationResult.Success;

        var displayName = validationContext?.DisplayName ?? "The field";
        var resolver = validationContext?.GetService(typeof(IValidationMessageResolver)) as IValidationMessageResolver;

        return new ValidationResult(
            resolver is not null
                ? resolver.GetMessage(result.ErrorCode, displayName, CultureInfo.CurrentUICulture)
                : IranDataAnnotationsLocalization.GetMessage(result.ErrorCode, displayName));
    }
}

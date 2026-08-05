using System.Globalization;
using IranValidator.Core.Results;

namespace IranValidator.Localization;

/// <summary>
/// Resolves localized validation error messages based on the error code,
/// property name, and target culture.
/// </summary>
public interface IValidationMessageResolver
{
    /// <summary>
    /// Returns a localized validation error message.
    /// </summary>
    /// <param name="errorCode">The validation error code.</param>
    /// <param name="propertyName">The name of the validated property (may be null).</param>
    /// <param name="culture">The target culture for localization. Null uses the default invariant fallback.</param>
    /// <returns>A localized error message string.</returns>
    string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture);
}

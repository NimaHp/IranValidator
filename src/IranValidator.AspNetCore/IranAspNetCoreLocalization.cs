using System.Globalization;
using IranValidator.Core.Results;
using IranValidator.DataAnnotations;
using IranValidator.Localization;

namespace IranValidator.AspNetCore;

/// <summary>
/// Localization entry point for the ASP.NET Core adapter.
/// Provides the <see cref="IValidationMessageResolver"/> that is registered in
/// DI by <c>AddIranValidation</c> (so DataAnnotations attributes resolve
/// localized messages through request services) and localized HTTP titles for
/// the validation error responses produced by the filter and middleware.
/// </summary>
public static class IranAspNetCoreLocalization
{
    /// <summary>
    /// Creates the default DI resolver: it delegates to the
    /// <see cref="IranDataAnnotationsLocalization"/> registry, so customizations
    /// made through <c>IranDataAnnotationsLocalization.Configure</c> apply to
    /// the ASP.NET Core DI path as well.
    /// </summary>
    public static IValidationMessageResolver CreateDefaultResolver() => new DataAnnotationsRegistryResolver();

    /// <summary>
    /// Gets the localized validation error title for the specified culture,
    /// or <see cref="CultureInfo.CurrentUICulture"/> when null.
    /// </summary>
    public static string GetTitle(CultureInfo? culture = null)
    {
        var target = culture ?? CultureInfo.CurrentUICulture;
        return string.Equals(target.TwoLetterISOLanguageName, "fa", StringComparison.OrdinalIgnoreCase)
            ? "خطای اعتبارسنجی"
            : "Validation Error";
    }

    private sealed class DataAnnotationsRegistryResolver : IValidationMessageResolver
    {
        public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
            => IranDataAnnotationsLocalization.GetMessage(errorCode, propertyName, culture);
    }
}

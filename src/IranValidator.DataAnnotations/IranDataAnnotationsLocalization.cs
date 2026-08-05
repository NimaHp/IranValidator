using System.Globalization;
using IranValidator.Core.Results;
using IranValidator.Localization;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Static localization entry point for DataAnnotations attributes in hosts
/// that do not provide a <see cref="IValidationMessageResolver"/> through the
/// validation context's service provider (non-ASP.NET Core hosts). Attributes
/// prefer a DI-provided resolver and fall back to this registry.
/// </summary>
public static class IranDataAnnotationsLocalization
{
    private static readonly ValidationMessageOptions Options = CreateDefaultOptions();

    private static ValidationMessageOptions CreateDefaultOptions()
    {
        var options = new ValidationMessageOptions();
        options.AddBuiltInResolvers();
        return options;
    }

    /// <summary>
    /// Customizes the static resolver registry (e.g. register a custom culture
    /// or replace a built-in resolver). Thread-safe.
    /// </summary>
    public static void Configure(Action<ValidationMessageOptions> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        configure(Options);
    }

    /// <summary>
    /// Resolves a localized message for <paramref name="errorCode"/> using the
    /// specified culture, or <see cref="CultureInfo.CurrentUICulture"/> when
    /// <paramref name="culture"/> is null.
    /// </summary>
    public static string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture = null)
    {
        var target = culture ?? CultureInfo.CurrentUICulture;
        return Options.GetResolver(target).GetMessage(errorCode, propertyName, target);
    }
}

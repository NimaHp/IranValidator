using System.Globalization;
using IranValidator.Core.Results;
using IranValidator.Localization;

namespace IranValidator.FluentValidation;

/// <summary>
/// Static localization entry point for the FluentValidation rule extensions.
/// FluentValidation rules are configured once at validator construction, but
/// messages are resolved at validation time, so per-request cultures
/// (<see cref="CultureInfo.CurrentUICulture"/>) are honored automatically.
/// Configure the registry with <see cref="Configure"/> to customize resolvers.
/// </summary>
public static class IranFluentValidationLocalization
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

using System.Globalization;

namespace IranValidator.Localization;

/// <summary>
/// Extension methods for <see cref="ValidationMessageOptions"/>.
/// </summary>
public static class ValidationMessageOptionsExtensions
{
    /// <summary>
    /// Registers the built-in English and Persian (Farsi) message resolvers for
    /// the invariant, "en", "en-US", "fa" and "fa-IR" cultures. Other cultures
    /// fall back to the <see cref="ValidationMessageOptions.DefaultCulture"/>.
    /// </summary>
    /// <param name="options">The options instance.</param>
    /// <returns>The same instance, for chaining.</returns>
    public static ValidationMessageOptions AddBuiltInResolvers(this ValidationMessageOptions options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        options.AddResolver(CultureInfo.InvariantCulture, new EnglishMessageResolver());
        options.AddResolver(CultureInfo.GetCultureInfo("en"), new EnglishMessageResolver());
        options.AddResolver(CultureInfo.GetCultureInfo("en-US"), new EnglishMessageResolver());
        options.AddResolver(CultureInfo.GetCultureInfo("fa"), new PersianMessageResolver());
        options.AddResolver(CultureInfo.GetCultureInfo("fa-IR"), new PersianMessageResolver());

        return options;
    }
}

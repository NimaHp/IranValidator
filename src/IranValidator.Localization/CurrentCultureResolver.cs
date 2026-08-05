using System.Globalization;
using IranValidator.Core.Results;

namespace IranValidator.Localization;

/// <summary>
/// An <see cref="IValidationMessageResolver"/> that resolves the culture-specific
/// resolver from <see cref="ValidationMessageOptions"/> on every invocation,
/// reading <see cref="CultureInfo.CurrentUICulture"/> at call time.
/// </summary>
/// <remarks>
/// This wrapper is safe to register as a singleton: because the culture is read
/// per call, per-request culture changes (e.g. ASP.NET Core
/// <c>RequestLocalization</c>) are honored without re-resolving from DI.
/// An explicitly provided culture argument always wins over
/// <see cref="CultureInfo.CurrentUICulture"/>.
/// </remarks>
public sealed class CurrentCultureResolver : IValidationMessageResolver
{
    private readonly ValidationMessageOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentCultureResolver"/> class.
    /// </summary>
    /// <param name="options">The options containing the registered resolvers.</param>
    public CurrentCultureResolver(ValidationMessageOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
    {
        var targetCulture = culture ?? CultureInfo.CurrentUICulture;
        return _options.GetResolver(targetCulture).GetMessage(errorCode, propertyName, targetCulture);
    }
}

using System.Collections.Concurrent;
using System.Globalization;

namespace IranValidator.Localization;

/// <summary>
/// Options for configuring IranValidator localization services.
/// Thread-safe: resolvers may be registered and resolved concurrently
/// (e.g. lazy configuration alongside validation), so reads and writes
/// to the resolver table are synchronized.
/// </summary>
public sealed class ValidationMessageOptions
{
    private readonly ConcurrentDictionary<CultureInfo, IValidationMessageResolver> _resolvers = new(CultureInfoComparer.Instance);

    /// <summary>
    /// Gets the default culture used when no specific resolver matches.
    /// Defaults to <see cref="CultureInfo.InvariantCulture"/> (English).
    /// </summary>
    public CultureInfo DefaultCulture { get; set; } = CultureInfo.InvariantCulture;

    /// <summary>
    /// Registers a message resolver for the specified culture.
    /// </summary>
    public ValidationMessageOptions AddResolver(CultureInfo culture, IValidationMessageResolver resolver)
    {
        if (culture is null)
            throw new ArgumentNullException(nameof(culture));

        if (resolver is null)
            throw new ArgumentNullException(nameof(resolver));

        _resolvers[culture] = resolver;
        return this;
    }

    /// <summary>
    /// Gets the registered resolver for the specified culture.
    /// Resolution order:
    /// 1. Exact culture match (e.g. "fa-IR");
    /// 2. Parent culture chain (e.g. "fa-IR" → "fa");
    /// 3. <see cref="DefaultCulture"/>;
    /// 4. <see cref="CultureInfo.InvariantCulture"/>;
    /// 5. Built-in English resolver.
    /// </summary>
    public IValidationMessageResolver GetResolver(CultureInfo? culture)
    {
        if (culture is not null)
        {
            if (_resolvers.TryGetValue(culture, out var resolver))
                return resolver;

            // Walk the parent chain (e.g. "de-AT" → "de") up to invariant culture.
            for (var current = culture.Parent;
                 current is not null && current != CultureInfo.InvariantCulture;
                 current = current.Parent)
            {
                if (_resolvers.TryGetValue(current, out var parentResolver))
                    return parentResolver;
            }
        }

        if (_resolvers.TryGetValue(DefaultCulture, out var defaultResolver))
            return defaultResolver;

        if (_resolvers.TryGetValue(CultureInfo.InvariantCulture, out var invariantResolver))
            return invariantResolver;

        return EnglishMessageResolverInstance.Value;
    }

    private static readonly Lazy<EnglishMessageResolver> EnglishMessageResolverInstance = new(() => new EnglishMessageResolver());

    /// <summary>
    /// CultureInfo comparer that matches by full culture name (e.g. "fa-IR"),
    /// case-insensitively. Parent-culture fallback is handled by
    /// <see cref="GetResolver"/>, not by this comparer.
    /// </summary>
    private sealed class CultureInfoComparer : IEqualityComparer<CultureInfo>
    {
        public static readonly CultureInfoComparer Instance = new();

        public bool Equals(CultureInfo? x, CultureInfo? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(CultureInfo obj) =>
            StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
    }
}

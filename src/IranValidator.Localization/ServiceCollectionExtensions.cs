using Microsoft.Extensions.DependencyInjection;

namespace IranValidator.Localization;

/// <summary>
/// Extension methods for registering IranValidator localization services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers IranValidator localization services with a default culture.
    /// Pre-registers English and Persian resolvers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action for custom resolvers.</param>
    public static IServiceCollection AddIranLocalization(
        this IServiceCollection services,
        Action<ValidationMessageOptions>? configure = null)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        var options = new ValidationMessageOptions();

        // Register built-in resolvers
        options.AddBuiltInResolvers();

        // User overrides
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IValidationMessageResolver>(sp =>
            new CurrentCultureResolver(sp.GetRequiredService<ValidationMessageOptions>()));

        return services;
    }
}

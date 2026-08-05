using Microsoft.Extensions.DependencyInjection;

namespace IranValidator.MinimalApis;

/// <summary>
/// Extension methods for registering IranValidator services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IranValidatorService"/> as a singleton for use in Minimal API endpoints.
    /// </summary>
    public static IServiceCollection AddIranValidator(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IranValidatorService>();
        return services;
    }
}

using IranValidator.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IranValidator.AspNetCore;

/// <summary>
/// Extension methods for registering IranValidator ASP.NET Core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers IranValidator services and configures MVC to use
    /// DataAnnotations-based validation with consistent error responses.
    /// </summary>
    public static IServiceCollection AddIranValidation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // DataAnnotations attributes (IranValidationAttribute) prefer a message
        // resolver from the validation context's service provider. Registering
        // it here makes validation messages localized in MVC model state.
        services.AddSingleton<IValidationMessageResolver>(_ => IranAspNetCoreLocalization.CreateDefaultResolver());

        services.Configure<ApiBehaviorOptions>(options =>
        {
            // Use custom validation response for invalid models
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(x => new
                    {
                        Field = e.Key,
                        Error = x.ErrorMessage
                    }));

                return new BadRequestObjectResult(new
                {
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = IranAspNetCoreLocalization.GetTitle(),
                    Status = StatusCodes.Status400BadRequest,
                    Errors = errors
                });
            };
        });

        return services;
    }
}

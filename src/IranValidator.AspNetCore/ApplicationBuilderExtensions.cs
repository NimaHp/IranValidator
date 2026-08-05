using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace IranValidator.AspNetCore;

/// <summary>
/// Extension methods for the IranValidator ASP.NET Core middleware pipeline.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a middleware that catches validation-related exceptions and returns
    /// structured Problem Details responses.
    /// </summary>
    public static IApplicationBuilder UseIranValidation(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            AllowStatusCode404Response = true,
            ExceptionHandler = async context =>
            {
                var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandler?.Error != null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/problem+json";

                    var problem = new
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        Title = IranAspNetCoreLocalization.GetTitle(),
                        Status = StatusCodes.Status400BadRequest,
                        Detail = exceptionHandler.Error.Message
                    };

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(problem));
                }
            }
        });

        return app;
    }
}

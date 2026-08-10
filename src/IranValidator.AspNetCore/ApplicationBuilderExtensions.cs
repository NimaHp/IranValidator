using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IranValidator.AspNetCore;

/// <summary>
/// Extension methods for the IranValidator ASP.NET Core middleware pipeline.
/// </summary>
public static partial class ApplicationBuilderExtensions
{
    [LoggerMessage(Level = LogLevel.Error, EventId = 1, Message = "Unhandled exception caught by UseIranValidation.")]
    private static partial void LogUnhandledException(ILogger logger, Exception exception);

    /// <summary>
    /// Adds a middleware that maps exceptions to structured Problem Details
    /// responses: <see cref="ValidationException"/> becomes 400 (the message
    /// is developer-authored and safe to show); any other exception becomes a
    /// generic 500 and is logged server-side — internal details are never
    /// leaked to the caller.
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
                var error = exceptionHandler?.Error;
                if (error is null)
                    return;

                // Validation failure → 400 with the developer-provided message.
                if (error is ValidationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/problem+json";

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(new
                        {
                            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                            Title = IranAspNetCoreLocalization.GetTitle(),
                            Status = StatusCodes.Status400BadRequest,
                            Detail = error.Message
                        }));
                    return;
                }

                // Unexpected exception → log it and return a generic 500;
                // never expose internal exception details to the client.
                var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();
                if (loggerFactory is not null)
                    LogUnhandledException(loggerFactory.CreateLogger("IranValidator.AspNetCore"), error);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(new
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        Title = "Internal Server Error",
                        Status = StatusCodes.Status500InternalServerError
                    }));
            }
        });

        return app;
    }
}

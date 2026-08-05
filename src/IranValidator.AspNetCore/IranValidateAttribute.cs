using Microsoft.AspNetCore.Mvc.Filters;

namespace IranValidator.AspNetCore;

/// <summary>
/// Action filter that validates Persian data using the DataAnnotations
/// attributes from IranValidator.DataAnnotations.
/// Works automatically with ASP.NET Core model binding when attributes
/// are applied to model properties.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class IranValidateAttribute : Attribute, IAsyncActionFilter
{
    /// <inheritdoc />
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // DataAnnotations validation is already handled by ASP.NET Core's
        // built-in model validation. This filter runs the check explicitly.
        if (!context.ModelState.IsValid)
        {
            context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
                new
                {
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = IranAspNetCoreLocalization.GetTitle(),
                    Status = Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest,
                    Errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors.Select(x => new
                        {
                            Field = e.Key,
                            Error = x.ErrorMessage
                        }))
                });
            return;
        }

        await next();
    }
}

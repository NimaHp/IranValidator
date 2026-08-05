using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using IranValidator.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace IranValidator.Tests.AspNetCore;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddIranValidation_RegistersOptions()
    {
        var services = new ServiceCollection();
        services.AddControllers();
        services.AddIranValidation();

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IConfigureOptions<ApiBehaviorOptions>>();

        options.Should().NotBeNull();
    }

    [Fact]
    public void AddIranValidation_ThrowsOnNull()
    {
        var act = () => ((IServiceCollection)null!).AddIranValidation();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddIranValidation_CanResolveOptions()
    {
        var services = new ServiceCollection();
        services.AddControllers();
        services.AddIranValidation();

        var provider = services.BuildServiceProvider();
        var apiOptions = provider.GetRequiredService<IOptions<ApiBehaviorOptions>>();

        apiOptions.Should().NotBeNull();
        apiOptions.Value.InvalidModelStateResponseFactory.Should().NotBeNull();
    }

    [Fact]
    public void AddIranValidation_InvalidModelStateFactory_ReturnsBadRequestWithErrors()
    {
        var services = new ServiceCollection();
        services.AddControllers();
        services.AddIranValidation();

        var provider = services.BuildServiceProvider();
        var apiOptions = provider.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value;

        var modelState = new ModelStateDictionary();
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        // In .NET 10, ActionContext.ModelState is a plain get-only auto-property
        // that MVC model binding sets at runtime — populate it via reflection.
        AnonymousAccessor.SetModelState(actionContext, modelState);
        modelState.AddModelError("name", "invalid");

        var result = apiOptions.InvalidModelStateResponseFactory(actionContext);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        AnonymousAccessor.Get<string>(badRequest.Value, "Title").Should().Be("Validation Error");
        AnonymousAccessor.Get<int>(badRequest.Value, "Status").Should().Be(400);
        var errors = (IEnumerable<object>)AnonymousAccessor.Get<object>(badRequest.Value, "Errors")!;
        errors.Should().ContainSingle();
        AnonymousAccessor.Get<string>(errors.First(), "Field").Should().Be("name");
    }

    [Fact]
    public void AddIranValidation_InvalidModelStateFactory_OmitsEntriesWithoutErrors()
    {
        var services = new ServiceCollection();
        services.AddControllers();
        services.AddIranValidation();

        var provider = services.BuildServiceProvider();
        var apiOptions = provider.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value;

        var modelState = new ModelStateDictionary();
        modelState.AddModelError("name", "invalid");
        modelState.SetModelValue("clean", "value", "value"); // bound without errors
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        // In .NET 10, ActionContext.ModelState is a plain get-only auto-property
        // that MVC model binding sets at runtime — populate it via reflection.
        AnonymousAccessor.SetModelState(actionContext, modelState);

        var result = apiOptions.InvalidModelStateResponseFactory(actionContext);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errors = (IEnumerable<object>)AnonymousAccessor.Get<object>(badRequest.Value, "Errors")!;
        errors.Should().ContainSingle();
        AnonymousAccessor.Get<string>(errors.First(), "Field").Should().Be("name");
    }

}

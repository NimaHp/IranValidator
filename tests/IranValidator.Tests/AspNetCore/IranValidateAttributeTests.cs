using FluentAssertions;
using IranValidator.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace IranValidator.Tests.AspNetCore;

public class IranValidateAttributeTests
{
    private static ActionExecutingContext CreateContext(Action<ModelStateDictionary> configureModelState)
    {
        var modelState = new ModelStateDictionary();
        configureModelState(modelState);

        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());
        // In .NET 10, ActionContext.ModelState is a plain get-only auto-property
        // that MVC model binding sets at runtime — populate it via reflection.
        AnonymousAccessor.SetModelState(actionContext, modelState);

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: new object());
    }

    private static Task<ActionExecutedContext> CompleteNext(ActionExecutingContext context, Action onCalled)
    {
        onCalled();
        return Task.FromResult(new ActionExecutedContext(
            context,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>()));
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvalidModelState_ReturnsBadRequestAndSkipsNext()
    {
        var context = CreateContext(m => m.AddModelError("name", "must not be empty"));
        var filter = new IranValidateAttribute();
        var nextCalled = false;

        await filter.OnActionExecutionAsync(context, () => CompleteNext(context, () => nextCalled = true));

        nextCalled.Should().BeFalse();
        var result = context.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        AnonymousAccessor.Get<string>(result.Value, "Title").Should().Be("Validation Error");
        AnonymousAccessor.Get<int>(result.Value, "Status").Should().Be(400);
        var errors = (IEnumerable<object>)AnonymousAccessor.Get<object>(result.Value, "Errors")!;
        errors.Should().HaveCount(1);
        AnonymousAccessor.Get<string>(errors.First(), "Field").Should().Be("name");
        AnonymousAccessor.Get<string>(errors.First(), "Error").Should().Be("must not be empty");
    }

    [Fact]
    public async Task OnActionExecutionAsync_ValidModelState_CallsNext()
    {
        var context = CreateContext(_ => { });
        var filter = new IranValidateAttribute();
        var nextCalled = false;

        await filter.OnActionExecutionAsync(context, () => CompleteNext(context, () => nextCalled = true));

        nextCalled.Should().BeTrue();
        context.Result.Should().BeNull();
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvalidModelState_OmitsEntriesWithoutErrors()
    {
        var context = CreateContext(m =>
        {
            m.AddModelError("name", "must not be empty");
            m.SetModelValue("clean", "value", "value"); // bound without errors
        });
        var filter = new IranValidateAttribute();

        await filter.OnActionExecutionAsync(context, () => CompleteNext(context, () => { }));

        var result = context.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errors = (IEnumerable<object>)AnonymousAccessor.Get<object>(result.Value, "Errors")!;
        errors.Should().ContainSingle();
        AnonymousAccessor.Get<string>(errors.First(), "Field").Should().Be("name");
    }
}

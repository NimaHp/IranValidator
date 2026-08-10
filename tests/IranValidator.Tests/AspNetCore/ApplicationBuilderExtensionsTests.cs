using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using IranValidator.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace IranValidator.Tests.AspNetCore;

public class ApplicationBuilderExtensionsTests
{
    [Fact]
    public void UseIranValidation_ThrowsOnNull()
    {
        var act = () => ((IApplicationBuilder)null!).UseIranValidation();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IranValidationFilter_HasAttributeUsage()
    {
        var attr = typeof(IranValidateAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false);
        attr.Should().HaveCount(1);
        var usage = (AttributeUsageAttribute)attr[0];
        usage.ValidOn.Should().HaveFlag(AttributeTargets.Class | AttributeTargets.Method);
    }

    [Fact]
    public void IranValidationFilter_CanInstantiate()
    {
        var filter = new IranValidateAttribute();
        filter.Should().NotBeNull();
        filter.Should().BeAssignableTo<Microsoft.AspNetCore.Mvc.Filters.IAsyncActionFilter>();
    }

    [Fact]
    public async Task UseIranValidation_WhenUnexpectedException_ReturnsGeneric500_WithoutLeakingMessage()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        var app = builder.Build();
        app.UseIranValidation();
        app.Run(_ => throw new InvalidOperationException("boom"));

        await app.StartAsync(TestContext.Current.CancellationToken);
        try
        {
            using var client = app.GetTestClient();
            using var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
            var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            body.Should().Contain("Internal Server Error");
            body.Should().NotContain("boom");
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
        }
    }

    [Fact]
    public async Task UseIranValidation_WhenValidationException_ReturnsBadRequestWithMessage()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        var app = builder.Build();
        app.UseIranValidation();
        app.Run(_ => throw new ValidationException("field is invalid"));

        await app.StartAsync(TestContext.Current.CancellationToken);
        try
        {
            using var client = app.GetTestClient();
            using var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
            var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            body.Should().Contain("field is invalid");
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
        }
    }

    [Fact]
    public async Task UseIranValidation_WhenNoException_PassesThrough()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        var app = builder.Build();
        app.UseIranValidation();
        app.Run(ctx => ctx.Response.WriteAsync("ok"));

        await app.StartAsync(TestContext.Current.CancellationToken);
        try
        {
            using var client = app.GetTestClient();
            using var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).Should().Be("ok");
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
        }
    }
}

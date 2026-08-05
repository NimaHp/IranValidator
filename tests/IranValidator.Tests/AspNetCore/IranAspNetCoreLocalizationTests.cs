using System.Globalization;
using FluentAssertions;
using IranValidator.AspNetCore;
using IranValidator.Core.Results;
using IranValidator.Localization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IranValidator.Tests.AspNetCore;

/// <summary>
/// Tests for the ASP.NET Core localization entry point: the DI-registered
/// resolver (delegating to the DataAnnotations registry) and localized titles.
/// </summary>
public class IranAspNetCoreLocalizationTests
{
    [Fact]
    public void GetTitle_DefaultCulture_ReturnsEnglish()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            IranAspNetCoreLocalization.GetTitle().Should().Be("Validation Error");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void GetTitle_PersianCulture_ReturnsPersian()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fa-IR");
            IranAspNetCoreLocalization.GetTitle().Should().Be("خطای اعتبارسنجی");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void GetTitle_ExplicitCulture_WinsOverCurrentUICulture()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fa-IR");
            IranAspNetCoreLocalization.GetTitle(CultureInfo.GetCultureInfo("en"))
                .Should().Be("Validation Error");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void CreateDefaultResolver_DelegatesToDataAnnotationsRegistry()
    {
        var resolver = IranAspNetCoreLocalization.CreateDefaultResolver();

        var message = resolver.GetMessage(
            ValidationErrorCode.InvalidChecksum, "Code", CultureInfo.GetCultureInfo("en"));

        message.Should().Be("Code has an invalid checksum.");
    }

    [Fact]
    public void CreateDefaultResolver_UsesPersianForFaCulture()
    {
        var resolver = IranAspNetCoreLocalization.CreateDefaultResolver();

        var message = resolver.GetMessage(
            ValidationErrorCode.InvalidLength, "Code", CultureInfo.GetCultureInfo("fa"));

        message.Should().Be("طول Code نامعتبر است.");
    }

    [Fact]
    public void AddIranValidation_RegistersMessageResolver()
    {
        var services = new ServiceCollection();
        services.AddIranValidation();

        var provider = services.BuildServiceProvider();
        provider.GetService<IValidationMessageResolver>().Should().NotBeNull();
    }
}

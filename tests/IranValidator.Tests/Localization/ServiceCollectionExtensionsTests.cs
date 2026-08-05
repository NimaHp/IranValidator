using System.Globalization;
using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Localization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IranValidator.Tests.Localization;

public class ServiceCollectionExtensionsTests
{
    private sealed class FrenchMessageResolver : IValidationMessageResolver
    {
        public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
            => "Message en français.";
    }

    [Fact]
    public void AddIranLocalization_WithoutConfig_ResolvesCurrentCultureWrapper()
    {
        var services = new ServiceCollection();
        services.AddIranLocalization();

        var provider = services.BuildServiceProvider();
        var resolver = provider.GetRequiredService<IValidationMessageResolver>();

        resolver.Should().NotBeNull();
        resolver.Should().BeOfType<CurrentCultureResolver>();
    }

    [Fact]
    public void AddIranLocalization_Singleton_HonorsCurrentUICulturePerCall()
    {
        var services = new ServiceCollection();
        services.AddIranLocalization();
        var provider = services.BuildServiceProvider();

        // Same singleton instance across culture switches — must honor culture per call.
        var resolver = provider.GetRequiredService<IValidationMessageResolver>();
        var before = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fa-IR");
            var persian = resolver.GetMessage(ValidationErrorCode.InvalidFormat, "Code", null);
            persian.Should().Be("فرمت Code نامعتبر است.");

            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var english = resolver.GetMessage(ValidationErrorCode.InvalidFormat, "Code", null);
            english.Should().Be("Code has an invalid format.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void AddIranLocalization_ExplicitCulture_WinsOverCurrentUICulture()
    {
        var services = new ServiceCollection();
        services.AddIranLocalization();
        var provider = services.BuildServiceProvider();

        var resolver = provider.GetRequiredService<IValidationMessageResolver>();
        var before = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var persian = resolver.GetMessage(
                ValidationErrorCode.InvalidFormat, "Code", CultureInfo.GetCultureInfo("fa-IR"));
            persian.Should().Be("فرمت Code نامعتبر است.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void AddIranLocalization_WithCustomCulture_ChangesResolver()
    {
        var services = new ServiceCollection();
        services.AddIranLocalization();

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<ValidationMessageOptions>();

        var persianResolver = options.GetResolver(CultureInfo.GetCultureInfo("fa-IR"));
        persianResolver.Should().BeOfType<PersianMessageResolver>();

        var englishResolver = options.GetResolver(CultureInfo.GetCultureInfo("en"));
        englishResolver.Should().BeOfType<EnglishMessageResolver>();
    }

    [Fact]
    public void AddIranLocalization_ThrowsOnNull()
    {
        var act = () => ((IServiceCollection)null!).AddIranLocalization();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddIranLocalization_WithConfigure_RegistersCustomResolver()
    {
        var services = new ServiceCollection();
        services.AddIranLocalization(options =>
            options.AddResolver(CultureInfo.GetCultureInfo("fr"), new FrenchMessageResolver()));
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<ValidationMessageOptions>();
        options.GetResolver(CultureInfo.GetCultureInfo("fr-FR")).Should().BeOfType<FrenchMessageResolver>();
    }
}

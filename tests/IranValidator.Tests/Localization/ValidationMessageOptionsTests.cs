using System.Globalization;
using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Localization;
using Xunit;

namespace IranValidator.Tests.Localization;

public class ValidationMessageOptionsTests
{
    private sealed class FrenchMessageResolver : IValidationMessageResolver
    {
        public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
            => "Message en français.";
    }

    [Fact]
    public void GetResolver_ExactCulture_ReturnsRegistered()
    {
        var options = new ValidationMessageOptions();
        options.AddResolver(CultureInfo.GetCultureInfo("fa-IR"), new PersianMessageResolver());

        var resolver = options.GetResolver(CultureInfo.GetCultureInfo("fa-IR"));
        resolver.Should().BeOfType<PersianMessageResolver>();
    }

    [Fact]
    public void GetResolver_NoExactMatch_WalksParentChain()
    {
        var options = new ValidationMessageOptions();
        options.AddResolver(CultureInfo.GetCultureInfo("fr"), new FrenchMessageResolver());

        // fr-FR has no exact match → falls back to parent culture "fr"
        var resolver = options.GetResolver(CultureInfo.GetCultureInfo("fr-FR"));
        resolver.Should().BeOfType<FrenchMessageResolver>();
    }

    [Fact]
    public void GetResolver_MultiLevelParentChain_ResolvesDeepAncestor()
    {
        var options = new ValidationMessageOptions();
        options.AddResolver(CultureInfo.GetCultureInfo("en"), new EnglishMessageResolver());

        // en-AU → en-GB? No: en-AU parent is "en" → found
        var resolver = options.GetResolver(CultureInfo.GetCultureInfo("en-AU"));
        resolver.Should().BeOfType<EnglishMessageResolver>();
    }

    [Fact]
    public void GetResolver_NoExactMatch_ReturnsDefaultOrInvariant()
    {
        var options = new ValidationMessageOptions();
        options.AddResolver(CultureInfo.InvariantCulture, new EnglishMessageResolver());

        // No resolver for fr-FR nor its parents → falls back to invariant
        var resolver = options.GetResolver(CultureInfo.GetCultureInfo("fr-FR"));
        resolver.Should().NotBeNull();
    }

    [Fact]
    public void GetResolver_DefaultCulture_PreferredOverInvariant()
    {
        var options = new ValidationMessageOptions();
        options.AddResolver(CultureInfo.InvariantCulture, new EnglishMessageResolver());
        options.AddResolver(CultureInfo.GetCultureInfo("fa"), new PersianMessageResolver());
        options.DefaultCulture = CultureInfo.GetCultureInfo("fa");

        // "tr-TR" has no match; neither its parents → DefaultCulture (fa)
        var resolver = options.GetResolver(CultureInfo.GetCultureInfo("tr-TR"));
        resolver.Should().BeOfType<PersianMessageResolver>();
    }

    [Fact]
    public void GetResolver_WhenNoResolverRegistered_ReturnsEnglishFallback()
    {
        var options = new ValidationMessageOptions();

        var resolver = options.GetResolver(null);
        resolver.Should().NotBeNull();
        resolver.Should().BeOfType<EnglishMessageResolver>();
    }

    [Fact]
    public void GetResolver_DefaultCultureUnregistered_FallsBackToInvariantResolver()
    {
        var options = new ValidationMessageOptions
        {
            // Default culture has NO resolver registered → invariant is next in chain
            DefaultCulture = CultureInfo.GetCultureInfo("fa")
        };
        options.AddResolver(CultureInfo.InvariantCulture, new EnglishMessageResolver());

        var resolver = options.GetResolver(CultureInfo.GetCultureInfo("tr-TR"));
        resolver.Should().BeOfType<EnglishMessageResolver>();
    }

    [Fact]
    public void AddResolver_NullCulture_Throws()
    {
        var options = new ValidationMessageOptions();

        var act = () => options.AddResolver(null!, new EnglishMessageResolver());

        act.Should().Throw<ArgumentNullException>().WithParameterName("culture");
    }

    [Fact]
    public void AddResolver_NullResolver_Throws()
    {
        var options = new ValidationMessageOptions();

        var act = () => options.AddResolver(CultureInfo.InvariantCulture, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("resolver");
    }

}

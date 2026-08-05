using System.Globalization;
using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.DataAnnotations;
using IranValidator.Localization;
using Xunit;

namespace IranValidator.Tests.DataAnnotations;

/// <summary>
/// Tests for the static DataAnnotations localization registry. Only additive
/// registrations (custom cultures) are used here so parallel test classes
/// never observe mutated built-in resolvers.
/// </summary>
public class IranDataAnnotationsLocalizationTests
{
    [Fact]
    public void GetMessage_DefaultCulture_ReturnsEnglish()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var message = IranDataAnnotationsLocalization.GetMessage(
                ValidationErrorCode.InvalidChecksum, "Code");
            message.Should().Be("Code has an invalid checksum.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void GetMessage_PersianCulture_ReturnsPersian()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fa-IR");
            var message = IranDataAnnotationsLocalization.GetMessage(
                ValidationErrorCode.InvalidChecksum, "Code");
            message.Should().Be("مجموع ارقام Code نامعتبر است.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void GetMessage_ExplicitCulture_WinsOverCurrentUICulture()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fa-IR");
            var message = IranDataAnnotationsLocalization.GetMessage(
                ValidationErrorCode.InvalidLength, "Code", CultureInfo.GetCultureInfo("en"));
            message.Should().Be("Code has an invalid length.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void GetMessage_UnregisteredCulture_FallsBackToEnglish()
    {
        var message = IranDataAnnotationsLocalization.GetMessage(
            ValidationErrorCode.InvalidFormat, "Code", CultureInfo.GetCultureInfo("de-DE"));
        message.Should().Be("Code has an invalid format.");
    }

    [Fact]
    public void Configure_AddsCustomCultureResolver()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
            IranDataAnnotationsLocalization.Configure(options =>
                options.AddResolver(CultureInfo.GetCultureInfo("fr"), new FrenchStubResolver()));

            var message = IranDataAnnotationsLocalization.GetMessage(
                ValidationErrorCode.InvalidFormat, "Code");
            message.Should().Be("Code [fr:InvalidFormat]");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void Configure_NullAction_Throws()
    {
        var act = () => IranDataAnnotationsLocalization.Configure(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("configure");
    }

    private sealed class FrenchStubResolver : IValidationMessageResolver
    {
        public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
            => $"{propertyName ?? "Value"} [fr:{errorCode}]";
    }
}

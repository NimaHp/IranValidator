using System.Globalization;
using FluentAssertions;
using FluentValidation;
using IranValidator.Core.Results;
using IranValidator.FluentValidation;
using IranValidator.Localization;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

/// <summary>
/// Verifies that FluentValidation rules resolve localized, code-specific
/// messages at validation time through <see cref="IranFluentValidationLocalization"/>.
/// </summary>
public class IranValidatorExtensionsLocalizationTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    [Fact]
    public void IranNationalCode_InvalidChecksum_EnglishMessage()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var validator = new InlineValidator<TestModel>();
            validator.RuleFor(x => x.Value).IranNationalCode();

            var result = validator.Validate(new TestModel { Value = "1234567890" });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Value has an invalid checksum.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void IranNationalCode_InvalidChecksum_PersianMessage_WhenCurrentUICultureIsPersian()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fa-IR");
            var validator = new InlineValidator<TestModel>();
            validator.RuleFor(x => x.Value).IranNationalCode();

            var result = validator.Validate(new TestModel { Value = "1234567890" });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "مجموع ارقام Value نامعتبر است.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void IranIban_UnknownBankCode_SpecificMessage()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var validator = new InlineValidator<TestModel>();
            validator.RuleFor(x => x.Value).IranIban();

            var result = validator.Validate(new TestModel { Value = "IR489991234567890123456789" });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Value has an invalid bank code.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void IranMobile_InvalidLength_MessageContainsPropertyName()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var validator = new InlineValidator<TestModel>();
            validator.RuleFor(x => x.Value).IranMobile();

            var result = validator.Validate(new TestModel { Value = "091212345678" });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("Value"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    [Fact]
    public void Configure_NullAction_Throws()
    {
        var act = () => IranFluentValidationLocalization.Configure(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("configure");
    }

    [Fact]
    public void Configure_AddsCustomCultureResolver()
    {
        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
            IranFluentValidationLocalization.Configure(options =>
                options.AddResolver(CultureInfo.GetCultureInfo("fr"), new FrenchStubResolver()));
            var validator = new InlineValidator<TestModel>();
            validator.RuleFor(x => x.Value).IranNationalCode();

            var result = validator.Validate(new TestModel { Value = "1234567890" });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Value [fr:InvalidChecksum]");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    private sealed class FrenchStubResolver : IValidationMessageResolver
    {
        public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
            => $"{propertyName ?? "Value"} [fr:{errorCode}]";
    }
}

using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranPassportValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("P12345678")]
    [InlineData("A12345678")]
    public void IranPassport_ValidValue_Passes(string passport)
    {
        _validator.RuleFor(x => x.Value).IranPassport();
        var result = _validator.Validate(new TestModel { Value = passport });
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IranPassport_Legacy8Digit_RequiresLocalOptIn()
    {
        var strict = new InlineValidator<TestModel>();
        strict.RuleFor(x => x.Value).IranPassport();
        var archive = new InlineValidator<TestModel>();
        archive.RuleFor(x => x.Value).IranPassport(allowLegacy8Digit: true);

        strict.Validate(new TestModel { Value = "12345678" }).IsValid.Should().BeFalse();
        archive.Validate(new TestModel { Value = "12345678" }).IsValid.Should().BeTrue();
        strict.Validate(new TestModel { Value = "12345678" }).IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("Z12345678")]
    [InlineData("1234567")]
    [InlineData("1234567890")]
    [InlineData("1234567A")]
    public void IranPassport_InvalidValue_Fails(string passport)
    {
        _validator.RuleFor(x => x.Value).IranPassport();
        var result = _validator.Validate(new TestModel { Value = passport });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranPassport_Archive_MalformedLegacy_Fails()
    {
        _validator.RuleFor(x => x.Value).IranPassport(allowLegacy8Digit: true);
        var result = _validator.Validate(new TestModel { Value = "1234567A" });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranPassport_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranPassport();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IranPassport_ErrorMessage_ContainsPropertyName()
    {
        _validator.RuleFor(x => x.Value).IranPassport();
        var result = _validator.Validate(new TestModel { Value = "123" });
        result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("Value"));
    }
}

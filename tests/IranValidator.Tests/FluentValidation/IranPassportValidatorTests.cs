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
    [InlineData("12345678")]
    public void IranPassport_ValidValue_Passes(string passport)
    {
        _validator.RuleFor(x => x.Value).IranPassport();
        var result = _validator.Validate(new TestModel { Value = passport });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Z12345678")]
    [InlineData("1234567")]
    [InlineData("1234567890")]
    public void IranPassport_InvalidValue_Fails(string passport)
    {
        _validator.RuleFor(x => x.Value).IranPassport();
        var result = _validator.Validate(new TestModel { Value = passport });
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

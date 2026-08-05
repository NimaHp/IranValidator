using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranMobileValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("09121234567")]
    [InlineData("09991234567")]
    [InlineData("09351234567")]
    public void IranMobile_ValidValue_Passes(string mobile)
    {
        _validator.RuleFor(x => x.Value).IranMobile();
        var result = _validator.Validate(new TestModel { Value = mobile });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("091212345678")]
    [InlineData("19121234567")]
    [InlineData("123")]
    public void IranMobile_InvalidValue_Fails(string mobile)
    {
        _validator.RuleFor(x => x.Value).IranMobile();
        var result = _validator.Validate(new TestModel { Value = mobile });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranMobile_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranMobile();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IranMobile_Empty_Passes()
    {
        _validator.RuleFor(x => x.Value).IranMobile();
        var result = _validator.Validate(new TestModel { Value = "" });
        result.IsValid.Should().BeTrue();
    }
}

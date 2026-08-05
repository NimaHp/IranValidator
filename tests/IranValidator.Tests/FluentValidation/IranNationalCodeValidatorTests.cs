using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranNationalCodeValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("0010350829")]
    [InlineData("9876543210")]
    [InlineData("2468013573")]
    public void IranNationalCode_ValidValue_Passes(string code)
    {
        _validator.RuleFor(x => x.Value).IranNationalCode();
        var result = _validator.Validate(new TestModel { Value = code });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("0000000000")]
    [InlineData("1234567890")]
    [InlineData("123")]
    public void IranNationalCode_InvalidValue_Fails(string code)
    {
        _validator.RuleFor(x => x.Value).IranNationalCode();
        var result = _validator.Validate(new TestModel { Value = code });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranNationalCode_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranNationalCode();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }
}

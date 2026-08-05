using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranEconomicCodeValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("123456789019")]
    [InlineData("987654321057")]
    [InlineData("005033968545")]
    public void IranEconomicCode_ValidValue_Passes(string code)
    {
        _validator.RuleFor(x => x.Value).IranEconomicCode();
        var result = _validator.Validate(new TestModel { Value = code });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("000000000000")]
    [InlineData("123456789012")]
    [InlineData("123")]
    public void IranEconomicCode_InvalidValue_Fails(string code)
    {
        _validator.RuleFor(x => x.Value).IranEconomicCode();
        var result = _validator.Validate(new TestModel { Value = code });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranEconomicCode_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranEconomicCode();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IranEconomicCode_ErrorMessage_ContainsPropertyName()
    {
        _validator.RuleFor(x => x.Value).IranEconomicCode();
        var result = _validator.Validate(new TestModel { Value = "123" });
        result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("Value"));
    }
}

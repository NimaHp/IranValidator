using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranPostalCodeValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("1234567890")]
    [InlineData("9876543210")]
    public void IranPostalCode_ValidValue_Passes(string postalCode)
    {
        _validator.RuleFor(x => x.Value).IranPostalCode();
        var result = _validator.Validate(new TestModel { Value = postalCode });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("0123456789")]
    [InlineData("12345")]
    [InlineData("12345678901")]
    public void IranPostalCode_InvalidValue_Fails(string postalCode)
    {
        _validator.RuleFor(x => x.Value).IranPostalCode();
        var result = _validator.Validate(new TestModel { Value = postalCode });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranPostalCode_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranPostalCode();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }
}

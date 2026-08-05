using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranCardNumberValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("6037991234567893")]   // Bank Melli Iran
    [InlineData("6104331234567890")]   // Bank Mellat
    [InlineData("5022291234567897")]   // Bank Pasargad
    public void IranCardNumber_ValidValue_Passes(string card)
    {
        _validator.RuleFor(x => x.Value).IranCardNumber();
        var result = _validator.Validate(new TestModel { Value = card });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("4539578763621486")]   // Luhn-valid VISA test number, not an Iranian BIN
    [InlineData("5500000000000004")]   // Luhn-valid MasterCard test number
    [InlineData("4539578763621487")]
    [InlineData("1234567890123456")]
    [InlineData("123")]
    public void IranCardNumber_InvalidValue_Fails(string card)
    {
        _validator.RuleFor(x => x.Value).IranCardNumber();
        var result = _validator.Validate(new TestModel { Value = card });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranCardNumber_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranCardNumber();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }
}

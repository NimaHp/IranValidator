using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranIbanValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("IR820540102680020817909002")]   // Bank Parsian (054)
    [InlineData("IR650111234567890123456789")]    // Bank Sanat & Madan (011)
    [InlineData("IR910121234567890123456789")]    // Bank Mellat (012)
    public void IranIban_ValidValue_Passes(string iban)
    {
        _validator.RuleFor(x => x.Value).IranIban();
        var result = _validator.Validate(new TestModel { Value = iban });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("IR489991234567890123456789")]    // valid checksum, unknown bank code (999)
    [InlineData("IR65011123456789012345678")]     // invalid length
    [InlineData("IR123456789012345678901234")]    // valid checksum, unknown bank code (123)
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWX")]      // invalid characters
    public void IranIban_InvalidValue_Fails(string iban)
    {
        _validator.RuleFor(x => x.Value).IranIban();
        var result = _validator.Validate(new TestModel { Value = iban });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranIban_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranIban();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IranIban_Empty_Passes()
    {
        _validator.RuleFor(x => x.Value).IranIban();
        var result = _validator.Validate(new TestModel { Value = "" });
        result.IsValid.Should().BeTrue();
    }
}

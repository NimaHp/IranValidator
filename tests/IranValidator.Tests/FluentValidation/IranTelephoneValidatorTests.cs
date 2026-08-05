using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranTelephoneValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("02122345678")]
    [InlineData("03132445678")]
    [InlineData("04132645678")]
    [InlineData("05138445678")]
    [InlineData("01133245678")]
    [InlineData("02632245678")]
    public void IranTelephone_ValidValue_Passes(string telephone)
    {
        _validator.RuleFor(x => x.Value).IranTelephone();
        var result = _validator.Validate(new TestModel { Value = telephone });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("02100000000")]   // local part starts with 0 (trunk prefix)
    [InlineData("02112345678")]   // local part starts with 1 (1xx = services)
    [InlineData("09121234567")]
    [InlineData("1234567")]
    public void IranTelephone_InvalidValue_Fails(string telephone)
    {
        _validator.RuleFor(x => x.Value).IranTelephone();
        var result = _validator.Validate(new TestModel { Value = telephone });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranTelephone_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranTelephone();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }
}

using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranVehiclePlateValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("12ب34567")]
    [InlineData("12ی34567")]
    [InlineData("12ب 345 67")]
    public void IranVehiclePlate_ValidValue_Passes(string plate)
    {
        _validator.RuleFor(x => x.Value).IranVehiclePlate();
        var result = _validator.Validate(new TestModel { Value = plate });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("12ب3456")]
    [InlineData("1AB34567")]
    [InlineData("12@34567")]
    public void IranVehiclePlate_InvalidValue_Fails(string plate)
    {
        _validator.RuleFor(x => x.Value).IranVehiclePlate();
        var result = _validator.Validate(new TestModel { Value = plate });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranVehiclePlate_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranVehiclePlate();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IranVehiclePlate_ErrorMessage_ContainsPropertyName()
    {
        _validator.RuleFor(x => x.Value).IranVehiclePlate();
        var result = _validator.Validate(new TestModel { Value = "12@" });
        result.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("Value"));
    }
}

using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace IranValidator.Tests.DataAnnotations;

public class IranVehiclePlateAttributeTests
{
    [Theory]
    [InlineData("12ب34567")]
    [InlineData("12ی34567")]
    [InlineData("12ب 345 67")]
    public void IsValid_ValidPlate_ReturnsSuccess(string plate)
    {
        var attr = new IranValidator.DataAnnotations.IranVehiclePlateAttribute();
        var result = attr.GetValidationResult(plate, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("12ب3456")]
    [InlineData("1AB34567")]
    [InlineData("12@34567")]
    [InlineData("")]
    public void IsValid_InvalidPlate_ReturnsError(string plate)
    {
        var attr = new IranValidator.DataAnnotations.IranVehiclePlateAttribute();
        var result = attr.GetValidationResult(plate, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranValidator.DataAnnotations.IranVehiclePlateAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonString_ReturnsError()
    {
        var attr = new IranValidator.DataAnnotations.IranVehiclePlateAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }
}

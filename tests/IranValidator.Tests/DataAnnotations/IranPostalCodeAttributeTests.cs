using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace IranValidator.Tests.DataAnnotations;

public class IranPostalCodeAttributeTests
{
    [Theory]
    [InlineData("1234567890")]
    [InlineData("9876543210")]
    [InlineData("1111111111")]
    public void IsValid_ValidPostalCode_ReturnsSuccess(string postalCode)
    {
        var attr = new IranValidator.DataAnnotations.IranPostalCodeAttribute();
        var result = attr.GetValidationResult(postalCode, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("0123456789")]
    [InlineData("12345")]
    [InlineData("12345678901")]
    [InlineData("abcdefghij")]
    [InlineData("")]
    public void IsValid_InvalidPostalCode_ReturnsError(string postalCode)
    {
        var attr = new IranValidator.DataAnnotations.IranPostalCodeAttribute();
        var result = attr.GetValidationResult(postalCode, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranValidator.DataAnnotations.IranPostalCodeAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonStringValue_ReturnsError()
    {
        var attr = new IranValidator.DataAnnotations.IranPostalCodeAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

}

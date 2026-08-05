using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace IranValidator.Tests.DataAnnotations;

public class IranEconomicCodeAttributeTests
{
    [Theory]
    [InlineData("123456789019")]
    [InlineData("987654321057")]
    [InlineData("005033968545")]
    public void IsValid_ValidEconomicCode_ReturnsSuccess(string economicCode)
    {
        var attr = new IranValidator.DataAnnotations.IranEconomicCodeAttribute();
        var result = attr.GetValidationResult(economicCode, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("000000000000")]
    [InlineData("123456789012")]
    [InlineData("123")]
    [InlineData("abcdefghijkl")]
    [InlineData("")]
    public void IsValid_InvalidEconomicCode_ReturnsError(string economicCode)
    {
        var attr = new IranValidator.DataAnnotations.IranEconomicCodeAttribute();
        var result = attr.GetValidationResult(economicCode, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranValidator.DataAnnotations.IranEconomicCodeAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonString_ReturnsError()
    {
        var attr = new IranValidator.DataAnnotations.IranEconomicCodeAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }
}

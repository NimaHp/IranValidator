using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace IranValidator.Tests.DataAnnotations;

public class NationalCodeAttributeTests
{
    [Theory]
    [InlineData("0010350829")]
    [InlineData("9876543210")]
    [InlineData("2468013573")]
    [InlineData("1234567891")]
    public void IsValid_ValidNationalCode_ReturnsSuccess(string code)
    {
        var attr = new IranValidator.DataAnnotations.NationalCodeAttribute();
        var result = attr.GetValidationResult(code, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("0000000000")]
    [InlineData("1234567890")]
    [InlineData("123")]
    [InlineData("abcdefghij")]
    [InlineData("")]
    public void IsValid_InvalidNationalCode_ReturnsError(string code)
    {
        var attr = new IranValidator.DataAnnotations.NationalCodeAttribute();
        var result = attr.GetValidationResult(code, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranValidator.DataAnnotations.NationalCodeAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonStringValue_ReturnsError()
    {
        var attr = new IranValidator.DataAnnotations.NationalCodeAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

}

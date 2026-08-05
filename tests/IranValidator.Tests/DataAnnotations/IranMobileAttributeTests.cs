using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace IranValidator.Tests.DataAnnotations;

public class IranMobileAttributeTests
{
    [Theory]
    [InlineData("09121234567")]
    [InlineData("09991234567")]
    [InlineData("09351234567")]
    [InlineData("09111234567")]
    public void IsValid_ValidMobile_ReturnsSuccess(string mobile)
    {
        var attr = new IranValidator.DataAnnotations.IranMobileAttribute();
        var result = attr.GetValidationResult(mobile, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("091212345678")]
    [InlineData("19121234567")]
    [InlineData("123")]
    [InlineData("abcdefghijk")]
    [InlineData("")]
    public void IsValid_InvalidMobile_ReturnsError(string mobile)
    {
        var attr = new IranValidator.DataAnnotations.IranMobileAttribute();
        var result = attr.GetValidationResult(mobile, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranValidator.DataAnnotations.IranMobileAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonStringValue_ReturnsError()
    {
        var attr = new IranValidator.DataAnnotations.IranMobileAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

}

using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace IranValidator.Tests.DataAnnotations;

public class IranTelephoneAttributeTests
{
    [Theory]
    [InlineData("02122345678")]
    [InlineData("03132445678")]
    [InlineData("04132645678")]
    [InlineData("05138445678")]
    [InlineData("06132245678")]
    [InlineData("07137245678")]
    [InlineData("08138245678")]
    [InlineData("01133245678")]
    [InlineData("08338245678")]
    public void IsValid_ValidTelephone_ReturnsSuccess(string telephone)
    {
        var attr = new IranValidator.DataAnnotations.IranTelephoneAttribute();
        var result = attr.GetValidationResult(telephone, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("09121234567")]
    [InlineData("1234567")]
    [InlineData("abcdefghij")]
    [InlineData("")]
    public void IsValid_InvalidTelephone_ReturnsError(string telephone)
    {
        var attr = new IranValidator.DataAnnotations.IranTelephoneAttribute();
        var result = attr.GetValidationResult(telephone, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranValidator.DataAnnotations.IranTelephoneAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonStringValue_ReturnsError()
    {
        var attr = new IranValidator.DataAnnotations.IranTelephoneAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

}

using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using IranValidator.DataAnnotations;
using Xunit;

namespace IranValidator.Tests.DataAnnotations;

public class IranIbanAttributeTests
{
    [Theory]
    [InlineData("IR820540102680020817909002")]   // Bank Parsian (054)
    [InlineData("IR650111234567890123456789")]    // Bank Sanat & Madan (011)
    [InlineData("IR910121234567890123456789")]    // Bank Mellat (012)
    public void IsValid_ValidIban_ReturnsSuccess(string iban)
    {
        var attr = new IranIbanAttribute();
        var result = attr.GetValidationResult(iban, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("IR489991234567890123456789")]    // valid checksum, unknown bank code (999)
    [InlineData("IR65011123456789012345678")]     // invalid length
    [InlineData("IR123456789012345678901234")]    // valid checksum, unknown bank code (123)
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWX")]      // invalid characters
    public void IsValid_InvalidIban_ReturnsError(string iban)
    {
        var attr = new IranIbanAttribute();
        var result = attr.GetValidationResult(iban, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranIbanAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonStringValue_ReturnsError()
    {
        var attr = new IranIbanAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }
}

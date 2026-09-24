using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;

namespace IranValidator.Tests.DataAnnotations;

public class IranPassportAttributeTests
{
    [Theory]
    [InlineData("P12345678")]
    [InlineData("A12345678")]
    public void IsValid_ValidPassport_ReturnsSuccess(string passport)
    {
        var attr = new IranValidator.DataAnnotations.IranPassportAttribute();
        var result = attr.GetValidationResult(passport, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Legacy8Digit_RequiresLocalOptIn()
    {
        var strict = new IranValidator.DataAnnotations.IranPassportAttribute();
        var archive = new IranValidator.DataAnnotations.IranPassportAttribute(allowLegacy8Digit: true);

        strict.GetValidationResult("12345678", new ValidationContext(new { })).Should().NotBe(ValidationResult.Success);
        archive.GetValidationResult("12345678", new ValidationContext(new { })).Should().Be(ValidationResult.Success);
        strict.GetValidationResult("12345678", new ValidationContext(new { })).Should().NotBe(ValidationResult.Success);
    }

    [Theory]
    [InlineData("Z12345678")]
    [InlineData("1234567")]
    [InlineData("1234567890")]
    [InlineData("AB1234567")]
    [InlineData("")]
    public void IsValid_InvalidPassport_ReturnsError(string passport)
    {
        var attr = new IranValidator.DataAnnotations.IranPassportAttribute();
        var result = attr.GetValidationResult(passport, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranValidator.DataAnnotations.IranPassportAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonString_ReturnsError()
    {
        var attr = new IranValidator.DataAnnotations.IranPassportAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }
}

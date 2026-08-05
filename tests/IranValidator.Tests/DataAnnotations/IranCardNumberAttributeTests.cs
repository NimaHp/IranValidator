using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace IranValidator.Tests.DataAnnotations;

public class IranCardNumberAttributeTests
{
    [Theory]
    [InlineData("6037991234567893")]   // Bank Melli Iran
    [InlineData("6104331234567890")]   // Bank Mellat
    [InlineData("5022291234567897")]   // Bank Pasargad
    [InlineData("6393461234567895")]   // Bank Sina
    public void IsValid_ValidCardNumber_ReturnsSuccess(string cardNumber)
    {
        var attr = new IranValidator.DataAnnotations.IranCardNumberAttribute();
        var result = attr.GetValidationResult(cardNumber, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("4539578763621486")]   // Luhn-valid VISA test number, not an Iranian BIN
    [InlineData("5500000000000004")]   // Luhn-valid MasterCard test number
    [InlineData("0000000000000000")]   // All zeros passes Luhn, no bank BIN
    [InlineData("4539578763621487")]
    [InlineData("1234567890123456")]
    [InlineData("123")]
    [InlineData("abcdefghijklmnop")]
    [InlineData("")]
    public void IsValid_InvalidCardNumber_ReturnsError(string cardNumber)
    {
        var attr = new IranValidator.DataAnnotations.IranCardNumberAttribute();
        var result = attr.GetValidationResult(cardNumber, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranValidator.DataAnnotations.IranCardNumberAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonStringValue_ReturnsError()
    {
        var attr = new IranValidator.DataAnnotations.IranCardNumberAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

}

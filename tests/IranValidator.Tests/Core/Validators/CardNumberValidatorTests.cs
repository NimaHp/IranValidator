using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class CardNumberValidatorTests
{
    private readonly CardNumberValidator _sut = CardNumberValidator.Instance;

    [Theory]
    [InlineData("6037991234567893")]   // Bank Melli Iran
    [InlineData("6104331234567890")]   // Bank Mellat
    [InlineData("5022291234567897")]   // Bank Pasargad
    [InlineData("6273531234567890")]   // Bank Tejarat
    [InlineData("5892101234567895")]   // Bank Sepah
    [InlineData("6393461234567895")]   // Bank Sina
    public void Validate_ValidIranianCards_ReturnsSuccess(string card)
    {
        var result = _sut.Validate(card);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("4539578763621486")]   // Luhn-valid VISA test number, not an Iranian BIN
    [InlineData("5500000000000004")]   // Luhn-valid MasterCard test number
    [InlineData("0000000000000000")]   // All zeros passes Luhn, no bank BIN
    public void Validate_NonIranianLuhnValidCards_ReturnsFailure(string card)
    {
        var result = _sut.Validate(card);
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.UnsupportedIssuer);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789012345")]    // 15 digits
    [InlineData("12345678901234567")]   // 17 digits
    [InlineData("6037991234567894")]    // Iranian BIN but wrong checksum
    [InlineData("abcd567890123456")]    // Contains letters
    [InlineData("1234 5678 9012 3456")] // With spaces
    public void Validate_InvalidCardNumbers_ReturnsFailure(string card)
    {
        var result = _sut.Validate(card);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_Null_ReturnsFailure()
    {
        _sut.Validate(null!).Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithSpaces_NormalizesCorrectly()
    {
        // "6037 9912 3456 7893" normalizes to a valid Melli card
        var result = _sut.Validate("6037 9912 3456 7893");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDashes_NormalizesCorrectly()
    {
        var result = _sut.Validate("6037-9912-3456-7893");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPersianDigits_NormalizesCorrectly()
    {
        var result = _sut.Validate("۶۰۳۷۹۹۱۲۳۴۵۶۷۸۹۳");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Instance_IsSingleton()
    {
        CardNumberValidator.Instance.Should().BeSameAs(CardNumberValidator.Instance);
    }
}

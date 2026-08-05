using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Extensions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class PostalCodeValidatorTests
{
    private readonly PostalCodeValidator _sut = PostalCodeValidator.Instance;

    [Theory]
    [InlineData("1234567890")]
    [InlineData("9876543210")]
    [InlineData("1111111111")]
    [InlineData("7777777777")]
    public void Validate_ValidPostalCode_ReturnsSuccess(string postalCode)
    {
        var result = _sut.Validate(postalCode.AsSpan());
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().NotBeNull();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    [Theory]
    [InlineData("0123456789")]
    [InlineData("12345")]
    [InlineData("12345678901")]
    [InlineData("abcdefghij")]
    public void Validate_InvalidPostalCode_ReturnsFailure(string postalCode)
    {
        var result = _sut.Validate(postalCode.AsSpan());
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().NotBe(ValidationErrorCode.None);
    }

    [Fact]
    public void Validate_EmptySpan_ReturnsFailure()
    {
        var result = _sut.Validate(ReadOnlySpan<char>.Empty);
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }

    [Fact]
    public void Validate_NullString_ReturnsFailure()
    {
        string? postalCode = null;
        var result = _sut.Validate(postalCode!);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_PersianDigits_ReturnsSuccess()
    {
        var result = _sut.Validate("۱۲۳۴۵۶۷۸۹۰");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDashAndSpace_ReturnsSuccess()
    {
        var result = _sut.Validate("123-456 7890");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void IsIranPostalCode_ReturnsTrue_ForValidPostalCode()
    {
        "1234567890".IsIranPostalCode().Should().BeTrue();
    }

    [Fact]
    public void IsIranPostalCode_ReturnsFalse_ForInvalidPostalCode()
    {
        "12345".IsIranPostalCode().Should().BeFalse();
    }

    [Fact]
    public void Instance_ReturnsSameReference()
    {
        var instance1 = PostalCodeValidator.Instance;
        var instance2 = PostalCodeValidator.Instance;
        instance1.Should().BeSameAs(instance2);
    }
}

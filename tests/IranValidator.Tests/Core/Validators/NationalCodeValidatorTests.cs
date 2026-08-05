using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Extensions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class NationalCodeValidatorTests
{
    private readonly NationalCodeValidator _sut = NationalCodeValidator.Instance;

    [Theory]
    [InlineData("0010350829")]
    [InlineData("1234567891")]
    [InlineData("9876543210")]
    [InlineData("2468013573")]
    public void Validate_ValidCodes_ReturnsSuccess(string code)
    {
        var result = _sut.Validate(code.AsSpan());
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().NotBeNull();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    [Theory]
    [InlineData("0000000000")]
    [InlineData("1111111111")]
    [InlineData("1234567890")]
    [InlineData("123")]
    [InlineData("12345678901")]
    [InlineData("abcdefghij")]
    public void Validate_InvalidCodes_ReturnsFailure(string code)
    {
        var result = _sut.Validate(code.AsSpan());
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
        string? code = null;
        var result = _sut.Validate(code!);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_PersianDigits_ReturnsSuccess()
    {
        var result = _sut.Validate("۰۰۱۰۳۵۰۸۲۹");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDashAndSpace_ReturnsSuccess()
    {
        var result = _sut.Validate("001-035 0829");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void IsIranNationalCode_ReturnsTrue_ForValidCode()
    {
        "0010350829".IsIranNationalCode().Should().BeTrue();
    }

    [Fact]
    public void IsIranNationalCode_ReturnsFalse_ForInvalidCode()
    {
        "1234567890".IsIranNationalCode().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranNationalCode_ReturnsValidationResult()
    {
        var result = "0010350829".ValidateIranNationalCode();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Instance_ReturnsSameReference()
    {
        var instance1 = NationalCodeValidator.Instance;
        var instance2 = NationalCodeValidator.Instance;
        instance1.Should().BeSameAs(instance2);
    }
}

using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Constants;
using IranValidator.Core.Extensions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class MobileValidatorTests
{
    private readonly MobileValidator _sut = MobileValidator.Instance;

    [Theory]
    [InlineData("09121234567")]
    [InlineData("09351234567")]
    [InlineData("09991234567")]
    [InlineData("09111234567")]
    [InlineData("09001234567")]
    [InlineData("09051234567")]
    [InlineData("09101234567")]
    [InlineData("09191234567")]
    [InlineData("09201234567")]
    [InlineData("09241234567")]
    [InlineData("09301234567")]
    [InlineData("09391234567")]
    [InlineData("09411234567")]
    [InlineData("09421234567")]
    [InlineData("09901234567")]
    [InlineData("09961234567")]
    [InlineData("09981234567")]
    public void Validate_ValidMobile_ReturnsSuccess(string mobile)
    {
        var result = _sut.Validate(mobile.AsSpan());
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().NotBeNull();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    [Theory]
    [InlineData("08121234567")]
    [InlineData("19121234567")]
    [InlineData("0912123456")]
    [InlineData("091212345678")]
    [InlineData("abcdefghijk")]
    [InlineData("09061234567")]
    [InlineData("09251234567")]
    [InlineData("09401234567")]
    [InlineData("09501234567")]
    [InlineData("09601234567")]
    [InlineData("09701234567")]
    [InlineData("09801234567")]
    [InlineData("09971234567")]
    public void Validate_InvalidMobile_ReturnsFailure(string mobile)
    {
        var result = _sut.Validate(mobile.AsSpan());
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
        string? mobile = null;
        var result = _sut.Validate(mobile!);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_PersianDigits_ReturnsSuccess()
    {
        var result = _sut.Validate("۰۹۱۲۱۲۳۴۵۶۷");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDashAndSpace_ReturnsSuccess()
    {
        var result = _sut.Validate("0912-123 4567");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void IsIranMobile_ReturnsTrue_ForValidMobile()
    {
        "09121234567".IsIranMobile().Should().BeTrue();
    }

    [Fact]
    public void IsIranMobile_ReturnsFalse_ForInvalidMobile()
    {
        "1234567890".IsIranMobile().Should().BeFalse();
    }

    [Fact]
    public void Instance_ReturnsSameReference()
    {
        var instance1 = MobileValidator.Instance;
        var instance2 = MobileValidator.Instance;
        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void MobilePrefixes_IsSortedUniqueAndWithinRange()
    {
        var prefixes = MobilePrefixes.Valid;
        prefixes.Should().BeInAscendingOrder();
        prefixes.Should().OnlyHaveUniqueItems();
        prefixes.Should().OnlyContain(p => p >= 900 && p <= 999);
        prefixes.Should().HaveCount(42);
    }
}

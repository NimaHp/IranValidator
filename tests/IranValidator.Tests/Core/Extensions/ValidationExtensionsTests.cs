using FluentAssertions;
using IranValidator.Core.Extensions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Extensions;

public class ValidationExtensionsTests
{
    // === Mobile Extensions ===

    [Fact]
    public void IsIranMobile_WithValidMobile_ReturnsTrue()
    {
        "09121234567".IsIranMobile().Should().BeTrue();
    }

    [Fact]
    public void IsIranMobile_WithInvalidMobile_ReturnsFalse()
    {
        "123".IsIranMobile().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranMobile_ReturnsValidationResult()
    {
        var result = "09121234567".ValidateIranMobile();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ValidateIranMobile_WithInvalidInput_ReturnsFailure()
    {
        var result = "123".ValidateIranMobile();
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().NotBe(ValidationErrorCode.None);
    }

    // === National Code Extensions ===

    [Fact]
    public void IsIranNationalCode_WithValidCode_ReturnsTrue()
    {
        "0010350829".IsIranNationalCode().Should().BeTrue();
    }

    [Fact]
    public void IsIranNationalCode_WithInvalidCode_ReturnsFalse()
    {
        "123".IsIranNationalCode().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranNationalCode_ReturnsValidationResult()
    {
        var result = "0010350829".ValidateIranNationalCode();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ValidateIranNationalCode_WithInvalidInput_ReturnsFailure()
    {
        var result = "0000000000".ValidateIranNationalCode();
        result.Success.Should().BeFalse();
    }

    // === Postal Code Extensions ===

    [Fact]
    public void IsIranPostalCode_WithValidCode_ReturnsTrue()
    {
        "1234567890".IsIranPostalCode().Should().BeTrue();
    }

    [Fact]
    public void IsIranPostalCode_WithInvalidCode_ReturnsFalse()
    {
        "123".IsIranPostalCode().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranPostalCode_ReturnsValidationResult()
    {
        var result = "1234567890".ValidateIranPostalCode();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ValidateIranPostalCode_WithInvalidInput_ReturnsFailure()
    {
        var result = "0123456789".ValidateIranPostalCode();
        result.Success.Should().BeFalse();
    }

    // === Telephone Extensions ===

    [Fact]
    public void IsIranTelephone_WithValidTelephone_ReturnsTrue()
    {
        "02122345678".IsIranTelephone().Should().BeTrue();
    }

    [Fact]
    public void IsIranTelephone_WithInvalidTelephone_ReturnsFalse()
    {
        "123".IsIranTelephone().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranTelephone_ReturnsValidationResult()
    {
        var result = "02122345678".ValidateIranTelephone();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ValidateIranTelephone_WithInvalidInput_ReturnsFailure()
    {
        var result = "021abcdefgh".ValidateIranTelephone();
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().NotBe(ValidationErrorCode.None);
    }

    // === IBAN Extensions ===

    [Fact]
    public void IsIranIban_WithValidIban_ReturnsTrue()
    {
        "IR820540102680020817909002".IsIranIban().Should().BeTrue();
    }

    [Fact]
    public void IsIranIban_WithInvalidIban_ReturnsFalse()
    {
        "123".IsIranIban().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranIban_ReturnsValidationResult()
    {
        var result = "IR820540102680020817909002".ValidateIranIban();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ValidateIranIban_WithInvalidInput_ReturnsFailure()
    {
        var result = "IR00000000000000000000000000".ValidateIranIban();
        result.Success.Should().BeFalse();
    }

    // === Card Number Extensions ===

    [Fact]
    public void IsIranCardNumber_WithValidCard_ReturnsTrue()
    {
        "6037991234567893".IsIranCardNumber().Should().BeTrue();
    }

    [Fact]
    public void IsIranCardNumber_WithInvalidCard_ReturnsFalse()
    {
        "123".IsIranCardNumber().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranCardNumber_ReturnsValidationResult()
    {
        var result = "6037991234567893".ValidateIranCardNumber();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ValidateIranCardNumber_WithInvalidInput_ReturnsFailure()
    {
        var result = "1234567890123456".ValidateIranCardNumber();
        result.Success.Should().BeFalse();
    }

    // === Company Id Extensions ===

    [Fact]
    public void IsIranCompanyId_WithValidCompanyId_ReturnsTrue()
    {
        "10380284795".IsIranCompanyId().Should().BeTrue();
    }

    [Fact]
    public void IsIranCompanyId_WithInvalidCompanyId_ReturnsFalse()
    {
        "123".IsIranCompanyId().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranCompanyId_ReturnsValidationResult()
    {
        var result = "10380284795".ValidateIranCompanyId();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ValidateIranCompanyId_WithInvalidInput_ReturnsFailure()
    {
        var result = "00000000000".ValidateIranCompanyId();
        result.Success.Should().BeFalse();
    }
}

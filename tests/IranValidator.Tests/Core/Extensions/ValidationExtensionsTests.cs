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
        "10380284752".IsIranCompanyId().Should().BeTrue();
    }

    [Fact]
    public void IsIranCompanyId_WithInvalidCompanyId_ReturnsFalse()
    {
        "123".IsIranCompanyId().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranCompanyId_ReturnsValidationResult()
    {
        var result = "10380284752".ValidateIranCompanyId();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ValidateIranCompanyId_WithInvalidInput_ReturnsFailure()
    {
        var result = "00000000000".ValidateIranCompanyId();
        result.Success.Should().BeFalse();
    }

    // === Economic Code Extensions ===

    [Theory]
    [InlineData("123456789019")]
    [InlineData("987654321057")]
    [InlineData("۱۲۳۴۵۶۷۸۹۰۱۹")]   // Persian digits
    public void IsIranEconomicCode_ValidCodes_ReturnsTrue(string code)
    {
        code.IsIranEconomicCode().Should().BeTrue();
    }

    [Theory]
    [InlineData("123456789012")]   // wrong checksum
    [InlineData("000000000000")]   // all same
    [InlineData("123")]
    [InlineData("")]
    [InlineData(null)]
    public void IsIranEconomicCode_InvalidCodes_ReturnsFalse(string? code)
    {
        code!.IsIranEconomicCode().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranEconomicCode_ValidCode_ReturnsSuccess()
    {
        var result = "123456789019".ValidateIranEconomicCode();
        result.Success.Should().BeTrue();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
        result.NormalizedValue.Should().Be("123456789019");
    }

    [Fact]
    public void ValidateIranEconomicCode_InvalidCode_ReturnsFailure()
    {
        var result = "123456789012".ValidateIranEconomicCode();
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().NotBe(ValidationErrorCode.None);
    }

    // === Passport Extensions ===

    [Theory]
    [InlineData("P12345678")]
    [InlineData("A12345678")]
    [InlineData("p12345678")]      // lowercase -> normalized
    public void IsIranPassport_ValidNumbers_ReturnsTrue(string passport)
    {
        passport.IsIranPassport().Should().BeTrue();
    }

    [Fact]
    public void IsIranPassport_Legacy8Digit_RequiresAllowLegacy()
    {
        // 8-digit legacy is InvalidFormat by default (1405); only passes with opt-in.
        "12345678".IsIranPassport().Should().BeFalse();
        bool prev = IranValidator.Core.Validators.PassportValidator.AllowLegacy8Digit;
        try
        {
            IranValidator.Core.Validators.PassportValidator.AllowLegacy8Digit = true;
            "12345678".IsIranPassport().Should().BeTrue();
        }
        finally { IranValidator.Core.Validators.PassportValidator.AllowLegacy8Digit = prev; }
    }

    [Theory]
    [InlineData("Z12345678")]      // invalid letter
    [InlineData("1234567")]        // too short
    [InlineData("1234567890")]     // too long
    [InlineData("")]
    [InlineData(null)]
    public void IsIranPassport_InvalidNumbers_ReturnsFalse(string? passport)
    {
        passport!.IsIranPassport().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranPassport_LowercaseLetter_NormalizesToUppercase()
    {
        var result = "p12345678".ValidateIranPassport();
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("P12345678");
    }

    [Fact]
    public void ValidateIranPassport_InvalidNumber_ReturnsFailure()
    {
        var result = "Z12345678".ValidateIranPassport();
        result.Success.Should().BeFalse();
    }

    // === Vehicle Plate Extensions ===

    [Theory]
    [InlineData("12ب34567")]
    [InlineData("12ی34567")]       // Persian letter
    [InlineData("12ب 345 67")]     // with spaces
    [InlineData("۱۲ب۳۴۵۶۷")]       // Persian digits
    public void IsIranVehiclePlate_ValidPlates_ReturnsTrue(string plate)
    {
        plate.IsIranVehiclePlate().Should().BeTrue();
    }

    [Theory]
    [InlineData("12ب3456")]        // too short
    [InlineData("1AB34567")]       // letter at wrong position
    [InlineData("12@34567")]       // invalid symbol
    [InlineData("12B34567")]       // Latin B is not an issued series letter
    [InlineData("")]
    [InlineData(null)]
    public void IsIranVehiclePlate_InvalidPlates_ReturnsFalse(string? plate)
    {
        plate!.IsIranVehiclePlate().Should().BeFalse();
    }

    [Fact]
    public void ValidateIranVehiclePlate_WithSpaces_ReturnsNormalized()
    {
        var result = "12ب 345 67".ValidateIranVehiclePlate();
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("12ب34567");
    }

    [Fact]
    public void ValidateIranVehiclePlate_InvalidPlate_ReturnsFailure()
    {
        var result = "12ب3456".ValidateIranVehiclePlate();
        result.Success.Should().BeFalse();
    }

    // === Null safety for economic/passport/plate extensions ===

    [Fact]
    public void AllEgPassportPlate_NullInput_ReturnFalse()
    {
        string? value = null;
        value!.IsIranEconomicCode().Should().BeFalse();
        value!.IsIranPassport().Should().BeFalse();
        value!.IsIranVehiclePlate().Should().BeFalse();
    }

    [Fact]
    public void AllEgPassportPlate_EmptyInput_ReturnFalse()
    {
        string value = "";
        value.IsIranEconomicCode().Should().BeFalse();
        value.IsIranPassport().Should().BeFalse();
        value.IsIranVehiclePlate().Should().BeFalse();
    }
}

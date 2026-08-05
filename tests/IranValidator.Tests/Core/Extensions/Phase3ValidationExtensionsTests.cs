using FluentAssertions;
using IranValidator.Core.Extensions;
using IranValidator.Core.Results;
using Xunit;

namespace IranValidator.Tests.Core.Extensions;

public class Phase3ValidationExtensionsTests
{
    // === Economic Code ===

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

    // === Passport ===

    [Theory]
    [InlineData("P12345678")]
    [InlineData("A12345678")]
    [InlineData("12345678")]
    [InlineData("p12345678")]      // lowercase → normalized
    public void IsIranPassport_ValidNumbers_ReturnsTrue(string passport)
    {
        passport.IsIranPassport().Should().BeTrue();
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

    // === Vehicle Plate ===

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

    // === Null safety for all extensions ===

    [Fact]
    public void AllExtensions_NullInput_ReturnFalse()
    {
        string? value = null;
        value!.IsIranEconomicCode().Should().BeFalse();
        value!.IsIranPassport().Should().BeFalse();
        value!.IsIranVehiclePlate().Should().BeFalse();
    }

    [Fact]
    public void AllExtensions_EmptyInput_ReturnFalse()
    {
        string value = "";
        value.IsIranEconomicCode().Should().BeFalse();
        value.IsIranPassport().Should().BeFalse();
        value.IsIranVehiclePlate().Should().BeFalse();
    }
}

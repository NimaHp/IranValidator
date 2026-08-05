using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class VehiclePlateValidatorTests
{
    private readonly VehiclePlateValidator _sut = VehiclePlateValidator.Instance;

    [Theory]
    [InlineData("12ب34567")]   // Tehran 12, personal series ب
    [InlineData("11ج48514")]   // J-series (ج)
    [InlineData("12د45678")]   // D-series (د)
    [InlineData("78س98765")]   // S-series (س)
    [InlineData("45و11223")]   // V-series (و)
    [InlineData("25ل45678")]   // L-series (ل)
    [InlineData("36م78945")]   // M-series (م)
    [InlineData("87ن23456")]   // N-series (ن)
    [InlineData("23پ56789")]   // police (پ)
    [InlineData("14ت78698")]   // taxi (ت)
    [InlineData("12ث34567")]   // IRGC (ث)
    [InlineData("16ا12345")]   // government (ا), Qom 16
    [InlineData("12ژ34567")]   // disabled/veterans (ژ)
    [InlineData("12ی34567")]   // Persian letter ی
    [InlineData("12D34567")]   // diplomatic (D)
    [InlineData("78S98765")]   // embassy service (S)
    [InlineData("12ب 345 67")] // with spaces (normalized)
    [InlineData("۱۲ب۳۴۵۶۷")]   // Persian digits (normalized)
    public void Validate_ValidPlates_ReturnsSuccess(string plate)
    {
        var result = _sut.Validate(plate.AsSpan());
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().NotBeNull();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    [Fact]
    public void Validate_PersianDigitsNormalized_ReturnsNormalized()
    {
        var result = _sut.Validate("۱۲ب۳۴۵۶۷");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("12ب34567");
    }

    [Fact]
    public void Validate_WithSpaces_NormalizesToCompact()
    {
        var result = _sut.Validate("12ب 345 67");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("12ب34567");
    }

    [Theory]
    [InlineData("12ب3456")]    // too short (7 chars)
    [InlineData("12ب345678")]  // too long (9 chars)
    [InlineData("1AB34567")]   // letter in digit position
    [InlineData("123ب4567")]   // letter at wrong position (3rd)
    [InlineData("12ب34ع67")]   // letter at wrong position (6th)
    [InlineData("12ب3456X")]   // letter at last position
    [InlineData("AB123456")]   // letters at start
    [InlineData("12@34567")]   // invalid symbol
    [InlineData("12 34567")]   // missing letter
    [InlineData("1_2ب34567")]  // underscore
    public void Validate_InvalidPlates_ReturnsFailure(string plate)
    {
        var result = _sut.Validate(plate.AsSpan());
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().NotBe(ValidationErrorCode.None);
    }

    // Letters that are NOT part of the official issuance series. Latin letters
    // other than D/S and Persian چ خ ر ح ض ظ are never printed on plates.
    [Theory]
    [InlineData("12B34567")]   // Latin B (not an official transliteration)
    [InlineData("12C34567")]   // Latin C
    [InlineData("12W34567")]   // Latin W
    [InlineData("12X34567")]   // Latin X
    [InlineData("12چ34567")]   // چ
    [InlineData("12خ34567")]   // خ
    [InlineData("12ر34567")]   // ر
    [InlineData("12ح34567")]   // ح
    [InlineData("12ض34567")]   // ض
    [InlineData("12ظ34567")]   // ظ
    public void Validate_UnissuedLetters_ReturnsInvalidFormat(string plate)
    {
        var result = _sut.Validate(plate);
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidFormat);
    }

    [Theory]
    [InlineData("12ب34539")]   // 39 — unused/reserved
    [InlineData("12ب34580")]   // 80 — unused/reserved
    [InlineData("12ب34590")]   // 90 — unused/reserved
    [InlineData("12ب34500")]   // 00 — never assigned
    [InlineData("12ب34509")]   // 09 — never assigned
    public void Validate_UnassignedProvinceCode_ReturnsInvalidProvinceCode(string plate)
    {
        var result = _sut.Validate(plate);
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidProvinceCode);
    }

    [Theory]
    [InlineData("12ب34511")]   // Tehran 11
    [InlineData("54د45678")]   // Yazd 54
    [InlineData("98س12345")]   // Ilam 98
    [InlineData("99ن88888")]   // Tehran 99
    [InlineData("68گ34567")]   // Alborz 68, temporary (گ)
    [InlineData("32ت34567")]   // Khorasan (shared 32), taxi (ت)
    public void Validate_AssignedProvinceCodes_ReturnsSuccess(string plate)
    {
        var result = _sut.Validate(plate);
        result.Success.Should().BeTrue();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
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
        var result = _sut.Validate((string)null!);
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }

    [Fact]
    public void Validate_InvalidLength_ReturnsInvalidLength()
    {
        var result = _sut.Validate("12ب3456");
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidLength);
    }

    [Fact]
    public void Validate_InvalidLetter_ReturnsInvalidFormat()
    {
        var result = _sut.Validate("12@34567");
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidFormat);
    }

    [Fact]
    public void Validate_InvalidCharacterInDigits_ReturnsInvalidCharacters()
    {
        var result = _sut.Validate("12ب34A67");
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidCharacters);
    }

    [Theory]
    [InlineData("12ب34567")]
    [InlineData("12ی34567")]
    public void Validate_StringOverload_ReturnsSuccess(string plate)
    {
        var result = _sut.Validate(plate);
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_ConcurrentAccess_NoRaceCondition()
    {
        var plates = new[] { "12ب34567", "11ج48514", "12د45678", "78س98765" };
        var bag = new System.Collections.Concurrent.ConcurrentBag<ValidationResult>();

        Parallel.For(0, 100, i =>
        {
            var result = _sut.Validate(plates[i % plates.Length]);
            bag.Add(result);
        });

        bag.Should().AllSatisfy(r => r.Success.Should().BeTrue());
    }
}

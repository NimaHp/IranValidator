using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class PassportValidatorTests
{
    private readonly PassportValidator _sut = PassportValidator.Instance;

    [Theory]
    [InlineData("P12345678")]   // standard new format - P series
    [InlineData("A12345678")]   // A series
    [InlineData("B12345678")]   // B series
    [InlineData("U12345678")]   // U series
    [InlineData("V12345678")]   // V series
    [InlineData("W12345678")]   // W series
    [InlineData("X12345678")]   // X series
    [InlineData("Y12345678")]   // Y series
    [InlineData("H12345678")]   // H series (diplomatic)
    [InlineData("F12345678")]   // F series
    [InlineData("00000000")]    // old format - 8 digits
    [InlineData("12345678")]    // old format
    [InlineData("p12345678")]   // lowercase letter (should normalize to uppercase)
    [InlineData("a12345678")]   // lowercase letter
    public void Validate_ValidFormats_ReturnsSuccess(string passport)
    {
        var result = _sut.Validate(passport.AsSpan());
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().NotBeNull();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    [Fact]
    public void Validate_LowercaseLetter_NormalizesToUppercase()
    {
        var result = _sut.Validate("p12345678");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("P12345678");
    }

    [Theory]
    [InlineData("")]           // empty
    [InlineData("1234567")]     // too short (7 chars)
    [InlineData("1234567890")]  // too long (10 chars)
    [InlineData("Z12345678")]   // invalid letter Z
    [InlineData("C12345678")]   // invalid letter C
    [InlineData("D12345678")]   // invalid letter D
    [InlineData("E12345678")]   // invalid letter E
    [InlineData("1234567A")]    // letter at wrong position (8-digit format)
    [InlineData("P1234567A")]   // letter at wrong position (9-digit format)
    [InlineData("AB1234567")]   // two letters
    [InlineData("PABCD5678")]   // letters in digit positions
    public void Validate_InvalidFormats_ReturnsFailure(string passport)
    {
        var result = _sut.Validate(passport.AsSpan());
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().NotBe(ValidationErrorCode.None);
    }

    [Fact]
    public void Validate_WithSpaces_NormalizesAndSucceeds()
    {
        var result = _sut.Validate("P 1234 5678");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("P12345678");
    }

    [Fact]
    public void Validate_EmptySpan_ReturnsFailure()
    {
        var result = _sut.Validate(ReadOnlySpan<char>.Empty);
        ResultShouldBeEmptyError(result);
    }

    [Fact]
    public void Validate_NullString_ReturnsFailure()
    {
        var result = _sut.Validate((string)null!);
        ResultShouldBeEmptyError(result);
    }

    [Fact]
    public void Validate_PersianDigitsNormalized_ReturnsSuccess()
    {
        var result = _sut.Validate("P۱۲۳۴۵۶۷۸");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("P12345678");
    }

    [Theory]
    [InlineData("P12345678")]
    [InlineData("12345678")]
    public void Validate_StringOverload_ReturnsSuccess(string value)
    {
        var result = _sut.Validate(value);
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_ConcurrentAccess_NoRaceCondition()
    {
        var passports = new[] { "P12345678", "A12345678", "12345678", "U12345678" };
        var bag = new System.Collections.Concurrent.ConcurrentBag<ValidationResult>();

        Parallel.For(0, 100, i =>
        {
            var result = _sut.Validate(passports[i % passports.Length]);
            bag.Add(result);
        });

        bag.Should().AllSatisfy(r => r.Success.Should().BeTrue());
    }

    private static void ResultShouldBeEmptyError(ValidationResult result)
    {
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }
}

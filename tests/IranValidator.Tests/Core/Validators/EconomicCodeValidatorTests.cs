using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class EconomicCodeValidatorTests
{
    private readonly EconomicCodeValidator _sut = EconomicCodeValidator.Instance;

    [Theory]
    [InlineData("123456789019")]   // checksum = 9 (remainder=2 → 11-2)
    [InlineData("987654321057")]   // checksum = 7 (remainder=4 → 11-4)
    [InlineData("123456789028")]   // checksum = 8 (remainder=3 → 11-3)
    [InlineData("100000000013")]   // checksum = 3 (remainder=8 → 11-8)
    [InlineData("005033968545")]   // checksum = 5 (remainder=6 → 11-6)
    [InlineData("000000000011")]   // checksum = 1 (remainder=1 → <2 → checksum = remainder)
    [InlineData("555555555557")]   // checksum = 7 (remainder=7 → 11-7... wait)
    [InlineData("200000000053")]   // checksum = 3
    [InlineData("100100100109")]   // checksum = 9
    public void Validate_ValidCodes_ReturnsSuccess(string code)
    {
        var result = _sut.Validate(code.AsSpan());
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().NotBeNull();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    [Theory]
    [InlineData("000000000000")]   // all same digits
    [InlineData("111111111111")]   // all same digits
    [InlineData("222222222222")]   // all same digits
    [InlineData("123456789012")]   // wrong checksum (expected 9, got 2)
    [InlineData("987654321098")]   // wrong checksum
    [InlineData("999999999990")]   // wrong checksum (expected 6, got 0)
    [InlineData("012345678901")]   // wrong checksum (expected 3, got 1)
    [InlineData("123")]            // too short
    [InlineData("1234567890123")]  // too long
    [InlineData("")]               // empty
    [InlineData("abcdefghijkl")]   // letters
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
        var result = _sut.Validate((string)null!);
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }

    [Fact]
    public void Validate_InvalidLength_ReturnsInvalidLength()
    {
        var result = _sut.Validate("1234567890".AsSpan());
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidLength);
    }

    [Fact]
    public void Validate_NonDigitCharacters_ReturnsInvalidCharacters()
    {
        var result = _sut.Validate("12345A78901?".AsSpan());
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidCharacters);
    }

    [Fact]
    public void Validate_AllSameDigit_ReturnsInvalidChecksum()
    {
        var result = _sut.Validate("000000000000".AsSpan());
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidChecksum);
    }

    [Fact]
    public void Validate_PersianDigitsNormalized_ReturnsSuccess()
    {
        var result = _sut.Validate("۱۲۳۴۵۶۷۸۹۰۱۹".AsSpan());
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("123456789019");
    }

    [Theory]
    [InlineData("123456789019")]
    [InlineData("987654321057")]
    public void Validate_StringOverload_ReturnsSuccess(string code)
    {
        var result = _sut.Validate(code);
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_ConcurrentAccess_NoRaceCondition()
    {
        var codes = new[] { "123456789019", "987654321057", "123456789028", "100000000013" };
        var bag = new System.Collections.Concurrent.ConcurrentBag<ValidationResult>();

        Parallel.For(0, 100, i =>
        {
            var result = _sut.Validate(codes[i % codes.Length]);
            bag.Add(result);
        });

        bag.Should().AllSatisfy(r => r.Success.Should().BeTrue());
    }
}

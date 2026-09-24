using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class PassportValidatorTests
{
    private readonly PassportValidator _sut = PassportValidator.Instance;

    private static ValidationResult ValidateWithLegacyIfNeeded(string passport)
    {
        var validator = passport.Length == 8 && passport.All(char.IsDigit)
            ? PassportValidator.CreateArchiveValidator()
            : PassportValidator.Instance;
        return validator.Validate(passport.AsSpan());
    }

    private static ValidationResult ValidateWithLegacyIfNeeded(ReadOnlySpan<char> passport)
        => ValidateWithLegacyIfNeeded(passport.ToString());

    [Theory]
    [InlineData("P12345678")]
    [InlineData("A12345678")]
    [InlineData("B12345678")]
    [InlineData("U12345678")]
    [InlineData("V12345678")]
    [InlineData("W12345678")]
    [InlineData("X12345678")]
    [InlineData("Y12345678")]
    [InlineData("H12345678")]
    [InlineData("F12345678")]
    [InlineData("00000000")]
    [InlineData("12345678")]
    [InlineData("p12345678")]
    [InlineData("a12345678")]
    public void Validate_ValidFormats_ReturnsSuccess(string passport)
    {
        var result = ValidateWithLegacyIfNeeded(passport.AsSpan());
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
    [InlineData("")]
    [InlineData("1234567")]
    [InlineData("1234567890")]
    [InlineData("Z12345678")]
    [InlineData("C12345678")]
    [InlineData("D12345678")]
    [InlineData("E12345678")]
    [InlineData("1234567A")]
    [InlineData("P1234567A")]
    [InlineData("AB1234567")]
    [InlineData("PABCD5678")]
    public void Validate_InvalidFormats_ReturnsFailure(string passport)
    {
        var result = _sut.Validate(passport.AsSpan());
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().NotBe(ValidationErrorCode.None);
    }

    [Fact]
    public void Validate_Archive_MalformedLegacy_ReturnsFailure()
    {
        var archive = PassportValidator.CreateArchiveValidator();

        foreach (var passport in new[] { "1234567A", "1234567#" })
        {
            var result = archive.Validate(passport);
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(ValidationErrorCode.InvalidCharacters);
        }
    }

    [Fact]
    public void Validate_StrictAndArchiveInstances_AreIndependent()
    {
        var strict = PassportValidator.Instance;
        var archive = PassportValidator.CreateArchiveValidator();

        strict.Validate("12345678").ErrorCode.Should().Be(ValidationErrorCode.InvalidFormat);
        archive.Validate("12345678").Success.Should().BeTrue();
        strict.Validate("12345678").ErrorCode.Should().Be(ValidationErrorCode.InvalidFormat);
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
        var result = ValidateWithLegacyIfNeeded(value);
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_ConcurrentStrictAndArchiveAccess_NoRaceCondition()
    {
        var strict = PassportValidator.Instance;
        var archive = PassportValidator.CreateArchiveValidator();
        var results = new System.Collections.Concurrent.ConcurrentBag<bool>();

        Parallel.For(0, 1000, i =>
        {
            var result = (i & 1) == 0
                ? strict.Validate("P12345678").Success
                : archive.Validate("12345678").Success;
            results.Add(result);
        });

        results.Should().HaveCount(1000);
        results.Should().AllBeEquivalentTo(true);
    }

    private static void ResultShouldBeEmptyError(ValidationResult result)
    {
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }
}

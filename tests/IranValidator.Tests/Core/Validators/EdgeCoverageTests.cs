using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

/// <summary>
/// Extra edge-case tests for uncovered branches.
/// </summary>
public class EdgeCoverageTests
{
    [Fact]
    public void MobileValidator_InvalidCharacterInDigits_ReturnsInvalidCharacters()
    {
        // 11 digits, starts with 09, valid operator (1), but has letter 'a'
        var result = MobileValidator.Instance.Validate("0912a456789");
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidCharacters);
    }

    [Fact]
    public void MobileValidator_AllZeros_Rejected()
    {
        // 11 zeros - invalid: operator digit must be 1-9
        var result = MobileValidator.Instance.Validate("00000000000");
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidFormat);
    }

    [Fact]
    public void NationalCodeValidator_WithSpaces_Rejected()
    {
        // 10 chars when normalized, but ' ' count... 
        // Actually spaces get removed by normalization, so "123 456 789" -> 9 chars
        var result = NationalCodeValidator.Instance.Validate("123 456 789");
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidLength);
    }
}

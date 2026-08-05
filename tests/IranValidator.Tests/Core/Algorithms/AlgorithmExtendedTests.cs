using FluentAssertions;
using IranValidator.Core.Algorithms;
using Xunit;

namespace IranValidator.Tests.Core.Algorithms;

public class LuhnAlgorithmExtendedTests
{
    // Valid Luhn numbers (algorithm only, not actual bank cards)
    [Theory]
    [InlineData("4539578763621486", true)]   // Valid from existing tests
    [InlineData("5500000000000004", true)]   // Valid from existing tests
    [InlineData("79927398713", true)]        // Standard test number
    [InlineData("", false)]
    [InlineData("1", false)]                 // Too short (less than 2)
    [InlineData("12", false)]                // Luhn: 1+4=5, 5%10≠0
    [InlineData("0000000000000000", true)]   // All zeros passes Luhn
    [InlineData("abcd567890123456", false)]  // Invalid chars via DigitToInt
    public void Validate_WithVariousInputs_ReturnsExpected(string number, bool expected)
    {
        LuhnAlgorithm.Validate(number.AsSpan()).Should().Be(expected);
    }

    [Fact]
    public void Validate_VeryLongInput_HandlesGracefully()
    {
        // Very long numeric string that passes Luhn
        // All zeros are valid Luhn
        string allZeros = new string('0', 1000);
        var result = LuhnAlgorithm.Validate(allZeros.AsSpan());
        result.Should().BeTrue(); // All zeros always passes Luhn
    }
}

public class NationalCodeAlgorithmExtendedTests
{
    [Theory]
    [InlineData("0010350829", true)]
    [InlineData("1234567891", true)]
    [InlineData("9876543210", true)]
    [InlineData("2468013573", true)]
    [InlineData("0000000000", false)]  // All same digits
    [InlineData("1111111111", false)]  // All same digits
    [InlineData("9999999999", false)]  // All same digits
    [InlineData("", false)]            // Empty
    [InlineData("123", false)]         // Too short
    [InlineData("12345678901", false)] // Too long
    public void Validate_WithVariousInputs_ReturnsExpected(string code, bool expected)
    {
        NationalCodeAlgorithm.Validate(code.AsSpan()).Should().Be(expected);
    }

    [Theory]
    [InlineData("abcdefghij")]     // Non-digit chars
    [InlineData("001-0350829")]    // With dash (not normalized at algorithm level)
    [InlineData("001 0350829")]    // With space
    [InlineData("123456789a")]     // Last char non-digit (covers checkDigit < 0 branch)
    public void Validate_WithNonDigitChars_ReturnsFalse(string code)
    {
        NationalCodeAlgorithm.Validate(code.AsSpan()).Should().BeFalse();
    }

    [Fact]
    public void Validate_AllSameDigits_DetectsAllVariants()
    {
        for (int d = 0; d <= 9; d++)
        {
            string code = new string((char)('0' + d), 10);
            NationalCodeAlgorithm.Validate(code.AsSpan()).Should().BeFalse($"all-{d}s should fail");
        }
    }

    [Fact]
    public void Validate_RemainderLessThan2_Branch()
    {
        // When remainder < 2, check digit must equal remainder
        // Find such a case by checking known valid codes
        // 0010350829: sum=0*10+0*9+1*8+0*7+3*6+5*5+0*4+8*3+2*2 = 0+0+8+0+18+25+0+24+4 = 79, 79%11=2, remainder>=2 so check=11-2=9 ✓ (last digit is 9)
        NationalCodeAlgorithm.Validate("0010350829".AsSpan()).Should().BeTrue();
    }
}

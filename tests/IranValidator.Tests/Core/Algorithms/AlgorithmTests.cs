using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Algorithms;
using Xunit;

namespace IranValidator.Tests.Core.Algorithms;

public class IbanAlgorithmTests
{
    [Theory]
    [InlineData("IR820540102680020817909002", true)]   // Valid Iranian IBAN
    [InlineData("ir820540102680020817909002", true)]   // Lowercase
    [InlineData("GB82WEST12345698765432", true)]       // UK test IBAN
    [InlineData("DE89370400440532013000", true)]        // German test IBAN
    [InlineData("IR820540102680020817909003", false)]   // Wrong check digit
    [InlineData("IR000000000000000000000000", false)]   // All zeros
    [InlineData("IR123456789012345678901234", false)]   // Random invalid
    [InlineData("", false)]                             // Empty
    [InlineData("IR12", false)]                         // Too short
    [InlineData("IR12345678901234567890123456", false)] // Wrong length
    [InlineData("XX820540102680020817909002", false)]   // Invalid country
    public void Validate_CorrectlyCalculates(string iban, bool expected)
    {
        IbanAlgorithm.Validate(iban.AsSpan()).Should().Be(expected);
    }

    [Fact]
    public void Validate_InvalidCharacters_ReturnsFalse()
    {
        IbanAlgorithm.Validate("IR82!@#$%^&*()".AsSpan()).Should().BeFalse();
    }
}

public class NationalCodeAlgorithmTests
{
    [Theory]
    [InlineData("0010350829", true)]
    [InlineData("1234567891", true)]
    [InlineData("9876543210", true)]
    [InlineData("2468013573", true)]
    [InlineData("0000000000", false)]
    [InlineData("1111111111", false)]
    [InlineData("1234567890", false)]
    public void Validate_CorrectlyCalculates(string code, bool expected)
    {
        NationalCodeAlgorithm.Validate(code.AsSpan()).Should().Be(expected);
    }
}

public class LuhnAlgorithmTests
{
    [Theory]
    [InlineData("4539578763621486", true)]
    [InlineData("5500000000000004", true)]
    [InlineData("6011514433546201", true)]
    [InlineData("4539578763621487", false)]
    [InlineData("1234567890", false)]
    public void Validate_CorrectlyCalculates(string number, bool expected)
    {
        LuhnAlgorithm.Validate(number.AsSpan()).Should().Be(expected);
    }
}

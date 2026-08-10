using System.Globalization;
using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Localization;
using Xunit;

namespace IranValidator.Tests.Localization;

public class EnglishMessageResolverTests
{
    private readonly EnglishMessageResolver _resolver = new();

    [Theory]
    [InlineData(ValidationErrorCode.ValueEmpty, "cannot be empty")]
    [InlineData(ValidationErrorCode.InvalidLength, "has an invalid length")]
    [InlineData(ValidationErrorCode.InvalidChecksum, "has an invalid checksum")]
    [InlineData(ValidationErrorCode.InvalidFormat, "has an invalid format")]
    [InlineData(ValidationErrorCode.InvalidCharacters, "contains invalid characters")]
    [InlineData(ValidationErrorCode.InvalidProvinceCode, "has an invalid province code")]
    [InlineData(ValidationErrorCode.InvalidBankCode, "has an invalid bank code")]
    [InlineData(ValidationErrorCode.InvalidAreaCode, "has an invalid area code")]
    [InlineData(ValidationErrorCode.UnsupportedIssuer, "is not issued by an Iranian bank")]
    [InlineData(ValidationErrorCode.ValueTooLarge, "is too long")]
    public void GetMessage_WithPropertyName_ReturnsFormattedMessage(ValidationErrorCode code, string expectedSuffix)
    {
        var msg = _resolver.GetMessage(code, "Mobile", null);
        msg.Should().Be($"Mobile {expectedSuffix}.");
    }

    [Fact]
    public void GetMessage_WithoutPropertyName_UsesValueAsDefault()
    {
        var msg = _resolver.GetMessage(ValidationErrorCode.InvalidFormat, null, null);
        msg.Should().Be("Value has an invalid format.");
    }

    [Fact]
    public void GetMessage_UnknownCode_ReturnsDefaultMessage()
    {
        var msg = _resolver.GetMessage((ValidationErrorCode)999, "Test", null);
        msg.Should().Be("Test is not valid.");
    }
}

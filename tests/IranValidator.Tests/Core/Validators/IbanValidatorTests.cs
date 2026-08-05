using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class IbanValidatorTests
{
    private readonly IbanValidator _sut = IbanValidator.Instance;

    [Theory]
    [InlineData("IR820540102680020817909002")]   // Bank Parsian (054)
    [InlineData("IR650111234567890123456789")]   // Bank of Industry and Mine (011)
    [InlineData("IR910121234567890123456789")]   // Bank Mellat (012)
    [InlineData("IR720151234567890123456789")]   // Bank Sepah (015)
    [InlineData("IR270171234567890123456789")]   // Bank Melli (017)
    [InlineData("IR530181234567890123456789")]   // Bank Tejarat (018)
    [InlineData("IR790191234567890123456789")]   // Bank Saderat (019)
    [InlineData("IR640521234567890123456789")]   // Ghavamin (052, merged into Sepah)
    [InlineData("IR970571234567890123456789")]   // Bank Pasargad (057)
    [InlineData("IR870791234567890123456789")]   // Mehr Eqtesad (079, merged into Sepah)
    [InlineData("IR160801234567890123456789")]   // Middle East Bank / Noor (080)
    [InlineData("IR180951234567890123456789")]   // Iran-Venezuela Bank (095)
    public void Validate_ValidIbans_ReturnsSuccess(string iban)
    {
        var result = _sut.Validate(iban);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("IR1234567890123456789012345")]    // Wrong length (25)
    [InlineData("IR123456789012345678901234567")]   // Wrong length (27)
    [InlineData("GB82WEST12345698765432")]           // Valid UK IBAN but not IR
    [InlineData("IR820540102680020817909003")]       // Wrong checksum (last digit differs)
    [InlineData("IR82054010268002081790900A")]       // Invalid char
    [InlineData("XX820540102680020817909002")]       // Invalid country code
    public void Validate_InvalidIbans_ReturnsFailure(string iban)
    {
        var result = _sut.Validate(iban);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_UnknownBankCode_ReturnsInvalidBankCode()
    {
        // Checksum-valid IBAN (ISO 7064 verified) with an unassigned bank
        // code 999 — MOD-97 alone cannot catch this.
        var result = _sut.Validate("IR489991234567890123456789");
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidBankCode);
    }

    [Fact]
    public void Validate_Null_ReturnsFailure()
    {
        _sut.Validate(null!).Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithSpaces_NormalizesCorrectly()
    {
        var result = _sut.Validate("IR82 0540 1026 8002 0817 9090 02");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDashes_NormalizesCorrectly()
    {
        var result = _sut.Validate("IR82-0540-1026-8002-0817-9090-02");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPersianDigitsInAccountPart_NormalizesCorrectly()
    {
        // Persian digits only in the account number part
        var result = _sut.Validate("IR82۰۵۴۰۱۰۲۶۸۰۰۲۰۸۱۷۹۰۹۰۰۲");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_Lowercase_ReturnsSuccess()
    {
        var result = _sut.Validate("ir820540102680020817909002");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_NoNormalizationNeeded_ReturnsOriginalReference()
    {
        const string value = "IR820540102680020817909002";
        var result = _sut.Validate(value);
        result.NormalizedValue.Should().BeSameAs(value);
    }

    [Fact]
    public void Validate_NormalizationNeeded_ReturnsNewNormalizedString()
    {
        var result = _sut.Validate("IR82 0540 1026 8002 0817 9090 02");
        result.NormalizedValue.Should().Be("IR820540102680020817909002");
    }

    [Fact]
    public void Instance_IsSingleton()
    {
        IbanValidator.Instance.Should().BeSameAs(IbanValidator.Instance);
    }
}

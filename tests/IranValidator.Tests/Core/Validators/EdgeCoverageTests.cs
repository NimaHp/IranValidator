using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

/// <summary>
/// Extra edge-case tests for uncovered branches.
/// </summary>
public class EdgeCoverageTests
{
    public static TheoryData<IStringValidator, string> ValidWithIranWord()
    {
        return new TheoryData<IStringValidator, string>
        {
            { NationalCodeValidator.Instance, "0010350829ایران" },
            { CompanyIdValidator.Instance, "10380284752ایران" },
            { EconomicCodeValidator.Instance, "123456789019ایران" },
            { MobileValidator.Instance, "09121234567ایران" },
            { PostalCodeValidator.Instance, "1234567890ایران" },
            { TelephoneValidator.Instance, "02122345678ایران" },
            { CardNumberValidator.Instance, "6037991234567893ایران" },
            { IbanValidator.Instance, "IR820540102680020817909002ایران" },
            { PassportValidator.Instance, "P12345678ایران" },
        };
    }

    [Theory]
    [MemberData(nameof(ValidWithIranWord))]
    public void IranWord_IsRejectedByNonPlateValidators(IStringValidator validator, string value)
    {
        validator.Validate(value).Success.Should().BeFalse();
    }

    [Fact]
    public void IranWord_IsAcceptedByVehiclePlateValidator()
    {
        VehiclePlateValidator.Instance.Validate("12ب345ایران67").Success.Should().BeTrue();
    }

    [Fact]
    public void BidiControls_AreRejectedByAllValidators()
    {
        var cases = new (IStringValidator Validator, string Value)[]
        {
            (NationalCodeValidator.Instance, "0010350829"),
            (CompanyIdValidator.Instance, "10380284752"),
            (EconomicCodeValidator.Instance, "123456789019"),
            (MobileValidator.Instance, "09121234567"),
            (PostalCodeValidator.Instance, "1234567890"),
            (TelephoneValidator.Instance, "02122345678"),
            (CardNumberValidator.Instance, "6037991234567893"),
            (IbanValidator.Instance, "IR820540102680020817909002"),
            (PassportValidator.Instance, "P12345678"),
            (VehiclePlateValidator.Instance, "12ب34567"),
        };

        foreach (var (validator, value) in cases)
            validator.Validate("\u202E" + value + "\u202C").Success.Should().BeFalse();
    }
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

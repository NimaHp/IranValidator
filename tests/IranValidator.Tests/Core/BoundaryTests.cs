using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core;

/// <summary>
/// Gate 6 — boundary tests: exact length sweeps (N−1/N/N+1), the empty-value
/// contract (null / empty / whitespace-only ⇒ <see cref="ValidationErrorCode.ValueEmpty"/>
/// for BOTH overloads), very long inputs, and the NormalizedValue contract.
/// </summary>
public sealed class ValidatorBoundaryTests
{
    // === Length sweeps: N−1 / N+1 ⇒ InvalidLength; N with bad content ⇒ specific code ===

    public static TheoryData<IStringValidator, string, ValidationErrorCode> InvalidCases()
    {
        var data = new TheoryData<IStringValidator, string, ValidationErrorCode>();

        // Mobile (11)
        data.Add(MobileValidator.Instance, "0912123456", ValidationErrorCode.InvalidLength);    // 10
        data.Add(MobileValidator.Instance, "091212345678", ValidationErrorCode.InvalidLength);  // 12
        data.Add(MobileValidator.Instance, "0912a456789", ValidationErrorCode.InvalidCharacters);
        data.Add(MobileValidator.Instance, "00000000000", ValidationErrorCode.InvalidFormat);   // operator digit 0
        data.Add(MobileValidator.Instance, "09061234567", ValidationErrorCode.InvalidFormat);  // unassigned prefix 0906

        // NationalCode (10)
        data.Add(NationalCodeValidator.Instance, "001035082", ValidationErrorCode.InvalidLength);   // 9
        data.Add(NationalCodeValidator.Instance, "00103508291", ValidationErrorCode.InvalidLength); // 11
        data.Add(NationalCodeValidator.Instance, "0000000000", ValidationErrorCode.InvalidChecksum);
        data.Add(NationalCodeValidator.Instance, "001035082A", ValidationErrorCode.InvalidCharacters);

        // PostalCode (10)
        data.Add(PostalCodeValidator.Instance, "123456789", ValidationErrorCode.InvalidLength);   // 9
        data.Add(PostalCodeValidator.Instance, "12345678901", ValidationErrorCode.InvalidLength); // 11
        data.Add(PostalCodeValidator.Instance, "0123456789", ValidationErrorCode.InvalidFormat);  // first digit 0
        data.Add(PostalCodeValidator.Instance, "123456789A", ValidationErrorCode.InvalidCharacters);

        // Telephone (11)
        data.Add(TelephoneValidator.Instance, "0212234567", ValidationErrorCode.InvalidLength);   // 10
        data.Add(TelephoneValidator.Instance, "021223456789", ValidationErrorCode.InvalidLength); // 12
        data.Add(TelephoneValidator.Instance, "01212345678", ValidationErrorCode.InvalidAreaCode);  // area code 12 not assigned
        data.Add(TelephoneValidator.Instance, "09112345678", ValidationErrorCode.InvalidAreaCode);  // mobile 09x is not a landline code
        data.Add(TelephoneValidator.Instance, "02100000000", ValidationErrorCode.InvalidFormat);  // local part starts with 0 (trunk prefix)
        data.Add(TelephoneValidator.Instance, "02112345678", ValidationErrorCode.InvalidFormat);  // local part starts with 1 (1xx = services)
        data.Add(TelephoneValidator.Instance, "0212234567A", ValidationErrorCode.InvalidCharacters);

        // CardNumber (16)
        data.Add(CardNumberValidator.Instance, "603799123456789", ValidationErrorCode.InvalidLength);   // 15
        data.Add(CardNumberValidator.Instance, "60379912345678912", ValidationErrorCode.InvalidLength); // 17
        data.Add(CardNumberValidator.Instance, "6037991234567890", ValidationErrorCode.InvalidChecksum);
        data.Add(CardNumberValidator.Instance, "0000000000000000", ValidationErrorCode.UnsupportedIssuer); // Luhn-valid, non-Iranian BIN
        data.Add(CardNumberValidator.Instance, "603799123456789A", ValidationErrorCode.InvalidCharacters);

        // Iban (26)
        data.Add(IbanValidator.Instance, "IR82054010268002081790902", ValidationErrorCode.InvalidLength);  // 25
        data.Add(IbanValidator.Instance, "IR8205401026800208179090021", ValidationErrorCode.InvalidLength); // 27
        data.Add(IbanValidator.Instance, "XX820540102680020817909002", ValidationErrorCode.InvalidFormat); // wrong country prefix
        data.Add(IbanValidator.Instance, "IR820540102680020817909000", ValidationErrorCode.InvalidChecksum);
        data.Add(IbanValidator.Instance, "IR82054010268002081790900A", ValidationErrorCode.InvalidChecksum); // non-digit → MOD-97 rejects

        // CompanyId (11)
        data.Add(CompanyIdValidator.Instance, "1038028479", ValidationErrorCode.InvalidLength);   // 10
        data.Add(CompanyIdValidator.Instance, "103802847951", ValidationErrorCode.InvalidLength); // 12
        data.Add(CompanyIdValidator.Instance, "1038028479A", ValidationErrorCode.InvalidCharacters);

        // EconomicCode (12)
        data.Add(EconomicCodeValidator.Instance, "12345678901", ValidationErrorCode.InvalidLength);   // 11
        data.Add(EconomicCodeValidator.Instance, "1234567890191", ValidationErrorCode.InvalidLength); // 13
        data.Add(EconomicCodeValidator.Instance, "000000000000", ValidationErrorCode.InvalidChecksum); // all-same digit
        data.Add(EconomicCodeValidator.Instance, "12345678901A", ValidationErrorCode.InvalidCharacters);

        // Passport (8 or 9)
        data.Add(PassportValidator.Instance, "1234567", ValidationErrorCode.InvalidLength);   // 7
        data.Add(PassportValidator.Instance, "P123456789", ValidationErrorCode.InvalidLength); // 10
        data.Add(PassportValidator.Instance, "Z12345678", ValidationErrorCode.InvalidFormat);  // letter not in valid set
        data.Add(PassportValidator.Instance, "P1234567A", ValidationErrorCode.InvalidCharacters);
        data.Add(PassportValidator.Instance, "1234567A", ValidationErrorCode.InvalidCharacters);

        // Iban (26): checksum-valid IBAN with unassigned bank code 999
        data.Add(IbanValidator.Instance, "IR489991234567890123456789", ValidationErrorCode.InvalidBankCode);

        // VehiclePlate (8)
        data.Add(VehiclePlateValidator.Instance, "12ب3456", ValidationErrorCode.InvalidLength);   // 7
        data.Add(VehiclePlateValidator.Instance, "12ب345678", ValidationErrorCode.InvalidLength); // 9
        data.Add(VehiclePlateValidator.Instance, "12@34567", ValidationErrorCode.InvalidFormat);  // invalid letter
        data.Add(VehiclePlateValidator.Instance, "12ب34A67", ValidationErrorCode.InvalidCharacters); // letter at digit position
        data.Add(VehiclePlateValidator.Instance, "1AB34567", ValidationErrorCode.InvalidCharacters); // letter at digit position
        data.Add(VehiclePlateValidator.Instance, "12ب34539", ValidationErrorCode.InvalidProvinceCode); // 39 unused/reserved

        return data;
    }

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void Validate_InvalidBoundary_ReturnsExpectedErrorCode(IStringValidator validator, string input, ValidationErrorCode expected)
    {
        ValidationResult result = validator.Validate(input);

        result.Success.Should().BeFalse($"\"{input}\" must be rejected");
        result.ErrorCode.Should().Be(expected);
    }

    // === Valid canonical samples + normalized-value contract ===

    public static TheoryData<IStringValidator, string, string> ValidCases()
    {
        var data = new TheoryData<IStringValidator, string, string>();

        data.Add(MobileValidator.Instance, "09121234567", "09121234567");
        data.Add(MobileValidator.Instance, "۰۹۱۲۱۲۳۴۵۶۷", "09121234567");   // Persian digits normalized
        data.Add(NationalCodeValidator.Instance, "0010350829", "0010350829");
        data.Add(PostalCodeValidator.Instance, "1234567890", "1234567890");
        data.Add(TelephoneValidator.Instance, "02122345678", "02122345678");
        data.Add(CardNumberValidator.Instance, "6037991234567893", "6037991234567893");
        data.Add(CardNumberValidator.Instance, "۶۰۳۷۹۹۱۲۳۴۵۶۷۸۹۳", "6037991234567893");
        data.Add(IbanValidator.Instance, "IR820540102680020817909002", "IR820540102680020817909002");
        data.Add(IbanValidator.Instance, "ir820540102680020817909002", "IR820540102680020817909002"); // lowercase prefix canonicalized
        data.Add(CompanyIdValidator.Instance, "10380284795", "10380284795");
        data.Add(EconomicCodeValidator.Instance, "123456789019", "123456789019");
        data.Add(PassportValidator.Instance, "12345678", "12345678");           // old format, 8 digits
        data.Add(PassportValidator.Instance, "P12345678", "P12345678");         // new format
        data.Add(PassportValidator.Instance, "p12345678", "P12345678");         // letter uppercased
        data.Add(VehiclePlateValidator.Instance, "12ب34567", "12ب34567");
        data.Add(VehiclePlateValidator.Instance, "۱۲ب۳۴۵۶۷", "12ب34567");      // Persian digits normalized

        return data;
    }

    [Theory]
    [MemberData(nameof(ValidCases))]
    public void Validate_ValidBoundary_ReturnsSuccessWithCanonicalValue(IStringValidator validator, string input, string expectedNormalized)
    {
        ValidationResult result = validator.Validate(input);

        result.Success.Should().BeTrue($"\"{input}\" must be accepted");
        result.NormalizedValue.Should().Be(expectedNormalized);
    }

    // === Empty-value contract: null / empty / whitespace-only ⇒ ValueEmpty (both overloads) ===

    public static IEnumerable<object[]> AllValidators()
    {
        yield return new object[] { MobileValidator.Instance };
        yield return new object[] { NationalCodeValidator.Instance };
        yield return new object[] { PostalCodeValidator.Instance };
        yield return new object[] { TelephoneValidator.Instance };
        yield return new object[] { CardNumberValidator.Instance };
        yield return new object[] { IbanValidator.Instance };
        yield return new object[] { CompanyIdValidator.Instance };
        yield return new object[] { EconomicCodeValidator.Instance };
        yield return new object[] { PassportValidator.Instance };
        yield return new object[] { VehiclePlateValidator.Instance };
    }

    [Theory]
    [MemberData(nameof(AllValidators))]
    public void Validate_NullEmptyWhitespace_ReturnsValueEmpty(IStringValidator validator)
    {
        validator.Validate((string)null!).ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
        validator.Validate(string.Empty).ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
        validator.Validate("   ").ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
        validator.Validate("\t\n").ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
        validator.Validate(" \u200F \u200E ").ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }

    [Theory]
    [MemberData(nameof(AllValidators))]
    public void Validate_SpanEmptyWhitespace_ReturnsValueEmpty(IStringValidator validator)
    {
        validator.Validate(ReadOnlySpan<char>.Empty).ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
        validator.Validate("   ".AsSpan()).ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
        validator.Validate("\u200F\u200E".AsSpan()).ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }

    // === Very long input: must fail fast and never throw ===

    [Theory]
    [MemberData(nameof(AllValidators))]
    public void Validate_VeryLongInput_ReturnsFailure_WithoutThrowing(IStringValidator validator)
    {
        var action = () =>
        {
            validator.Validate(new string('0', 100_000)).Success.Should().BeFalse();
            validator.Validate(new string(' ', 100_000)).ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
            validator.Validate(new string('1', 100_000) + "A").Success.Should().BeFalse();
        };

        action.Should().NotThrow();
    }

    // === Failure results never carry a normalized value ===

    [Theory]
    [MemberData(nameof(InvalidCases))]
    public void Validate_Failure_NormalizedValueIsNull(IStringValidator validator, string input, ValidationErrorCode _)
    {
        ValidationResult result = validator.Validate(input);
        result.NormalizedValue.Should().BeNull();
    }
}

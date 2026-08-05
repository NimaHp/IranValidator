using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Algorithms;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

/// <summary>
/// Boundary tests for the ReadOnlySpan&lt;char&gt; overloads of every validator.
/// The span overloads have their own empty-input guard and fast path that
/// the string overloads share via ValidateCore — these lines are not hit by
/// string-only tests.
/// </summary>
public class SpanOverloadTests
{
    public static TheoryData<IStringValidator, string> ValidSamples => new()
    {
        { NationalCodeValidator.Instance, "0010350829" },
        { CompanyIdValidator.Instance, "10380284795" },
        { EconomicCodeValidator.Instance, "005033968545" },
        { MobileValidator.Instance, "09121234567" },
        { PostalCodeValidator.Instance, "1145687654" },
        { TelephoneValidator.Instance, "02122345678" },
        { CardNumberValidator.Instance, "6037991234567893" },
        { IbanValidator.Instance, "IR820540102680020817909002" },
        { PassportValidator.Instance, "P12345678" },
        { VehiclePlateValidator.Instance, "12ب34567" },
    };

    public static TheoryData<IStringValidator> AllValidators => new()
    {
        { NationalCodeValidator.Instance },
        { CompanyIdValidator.Instance },
        { EconomicCodeValidator.Instance },
        { MobileValidator.Instance },
        { PostalCodeValidator.Instance },
        { TelephoneValidator.Instance },
        { CardNumberValidator.Instance },
        { IbanValidator.Instance },
        { PassportValidator.Instance },
        { VehiclePlateValidator.Instance },
    };

    [Theory]
    [MemberData(nameof(AllValidators))]
    public void Validate_SpanEmpty_ReturnsValueEmpty(IStringValidator validator)
    {
        var result = validator.Validate(ReadOnlySpan<char>.Empty);

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }

    [Theory]
    [MemberData(nameof(ValidSamples))]
    public void Validate_SpanValidInput_ReturnsSuccess(IStringValidator validator, string valid)
    {
        var result = validator.Validate(valid.AsSpan());

        result.Success.Should().BeTrue();
    }

    [Fact]
    public void TelephoneValidator_SpanFirstDigitNotZero_ReturnsInvalidFormat()
    {
        // 11 digits, first digit != '0' → format error (second-digit range check
        // is never reached, exercising the first-digit guard directly)
        var result = TelephoneValidator.Instance.Validate("12123456789".AsSpan());

        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidFormat);
    }

    [Fact]
    public void IbanValidator_InvalidCharacterInFirstFourChars_ReturnsFailure()
    {
        // '#' survives normalization (not a dash/space/digit/mark) and lands in
        // the country-code + check-digit section, hitting the first-4-char loop
        // of the MOD-97 algorithm.
        var result = IbanValidator.Instance.Validate("IR#01234567890123456789012");

        result.Success.Should().BeFalse();
    }
}

/// <summary>
/// Direct boundary tests for internal checksum algorithms. These branches are
/// defensive: the public validators pre-validate length and character classes,
/// so the algorithm-level guards are only reachable in isolation.
/// </summary>
public class AlgorithmBoundaryTests
{
    [Theory]
    [InlineData("123")]            // too short
    [InlineData("1234567890123")]  // too long
    public void EconomicCodeAlgorithm_WrongLength_ReturnsFalse(string code)
    {
        EconomicCodeAlgorithm.Validate(code.AsSpan()).Should().BeFalse();
    }

    [Fact]
    public void EconomicCodeAlgorithm_NonDigitInBody_ReturnsFalse()
    {
        EconomicCodeAlgorithm.Validate("A23456789012".AsSpan()).Should().BeFalse();
    }

    [Fact]
    public void EconomicCodeAlgorithm_NonDigitCheckDigit_ReturnsFalse()
    {
        EconomicCodeAlgorithm.Validate("12345678901A".AsSpan()).Should().BeFalse();
    }
}

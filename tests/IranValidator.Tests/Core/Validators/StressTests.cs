using System.Collections.Concurrent;
using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Constants;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

/// <summary>
/// Stress and edge-case tests: concurrent access, very long input, RTL, Unicode, etc.
/// </summary>
public class ValidatorStressTests
{
    // === Concurrent access (thread-safety) ===

    [Fact]
    public void MobileValidator_MultipleThreads_Succeeds()
    {
        var validator = MobileValidator.Instance;
        var results = new ConcurrentBag<bool>();

        Parallel.For(0, 100, _ =>
        {
            var r1 = validator.Validate("09121234567");
            var r2 = validator.Validate("invalid");
            results.Add(r1.Success);
            results.Add(!r2.Success);
        });

        results.Should().AllBeEquivalentTo(true);
    }

    [Fact]
    public void NationalCodeValidator_MultipleThreads_Succeeds()
    {
        var validator = NationalCodeValidator.Instance;
        var results = new ConcurrentBag<bool>();

        Parallel.For(0, 100, _ =>
        {
            results.Add(validator.Validate("0010350829").Success);
            results.Add(!validator.Validate("0000000000").Success);
        });

        results.Should().AllBeEquivalentTo(true);
    }

    [Fact]
    public void PostalCodeValidator_MultipleThreads_Succeeds()
    {
        var validator = PostalCodeValidator.Instance;
        var results = new ConcurrentBag<bool>();

        Parallel.For(0, 100, _ =>
        {
            results.Add(validator.Validate("1234567890").Success);
            results.Add(!validator.Validate("0123456789").Success);
        });

        results.Should().AllBeEquivalentTo(true);
    }

    // === Very long input ===

    [Fact]
    public void MobileValidator_VeryLongInput_DoesNotThrow()
    {
        var validator = MobileValidator.Instance;
        var veryLong = new string('0', 10000);
        var result = validator.Validate(veryLong);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void NationalCodeValidator_VeryLongInput_DoesNotThrow()
    {
        var validator = NationalCodeValidator.Instance;
        var veryLong = new string('0', 10000);
        var result = validator.Validate(veryLong);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void PostalCodeValidator_VeryLongInput_DoesNotThrow()
    {
        var validator = PostalCodeValidator.Instance;
        var veryLong = new string('0', 10000);
        var result = validator.Validate(veryLong);
        result.Success.Should().BeFalse();
    }

    // === RTL / Unicode marks ===

    [Fact]
    public void MobileValidator_WithRTLMarks_NormalizesCorrectly()
    {
        // "0912 123 4567" with RTL mark before
        var result = MobileValidator.Instance.Validate("\u200F09121234567");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void MobileValidator_WithBidiOverrides_NormalizesCorrectly()
    {
        var result = MobileValidator.Instance.Validate("\u202E09121234567\u202C");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void NationalCodeValidator_WithRTLMarks_NormalizesCorrectly()
    {
        var result = NationalCodeValidator.Instance.Validate("\u200F0010350829");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void PostalCodeValidator_WithZeroWidthChars_NormalizesCorrectly()
    {
        var result = PostalCodeValidator.Instance.Validate("123\u200B456\u200C7890");
        result.Success.Should().BeTrue();
    }

    // === Mixed Persian/Arabic/Latin digits ===

    [Fact]
    public void MobileValidator_MixedDigits_NormalizesCorrectly()
    {
        // Persian ۰۹, Arabic ١٢, Latin 34567 -> normalizes to 091234567 which is 9 digits not 11
        // Use full 11-digit: Persian ۰۹۱۲, Arabic ٣٤, Latin 56789
        var result = MobileValidator.Instance.Validate("۰۹۱٢٣٤56789");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void NationalCodeValidator_MixedDigits_NormalizesCorrectly()
    {
        // Persian and Arabic digits mixed
        var result = NationalCodeValidator.Instance.Validate("۰۰١۰۳۵۰۸۲۹");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void PostalCodeValidator_MixedDigits_NormalizesCorrectly()
    {
        var result = PostalCodeValidator.Instance.Validate("۱۲۳۴٥٦۷۸۹۰");
        result.Success.Should().BeTrue();
    }

    // === Null/Empty edge cases ===

    [Fact]
    public void AllValidators_NullInput_ReturnsFailure()
    {
        string? nullStr = null;
        MobileValidator.Instance.Validate(nullStr!).Success.Should().BeFalse();
        NationalCodeValidator.Instance.Validate(nullStr!).Success.Should().BeFalse();
        PostalCodeValidator.Instance.Validate(nullStr!).Success.Should().BeFalse();
    }

    [Fact]
    public void AllValidators_EmptyString_ReturnsFailure()
    {
        MobileValidator.Instance.Validate("").Success.Should().BeFalse();
        NationalCodeValidator.Instance.Validate("").Success.Should().BeFalse();
        PostalCodeValidator.Instance.Validate("").Success.Should().BeFalse();
    }

    [Fact]
    public void AllValidators_WhitespaceOnly_ReturnsFailure()
    {
        MobileValidator.Instance.Validate("   ").Success.Should().BeFalse();
        NationalCodeValidator.Instance.Validate("   ").Success.Should().BeFalse();
        PostalCodeValidator.Instance.Validate("   ").Success.Should().BeFalse();
    }

    // === Stress: many variations ===

    [Fact]
    public void MobileValidator_AllAssignedPrefixes_AreValid()
    {
        // Every assigned 09XX prefix (the MobilePrefixes list) must validate.
        foreach (ushort prefix in MobilePrefixes.Valid)
        {
            string mobile = $"{prefix:0000}0000000";
            var result = MobileValidator.Instance.Validate(mobile);
            result.Success.Should().BeTrue($"prefix {prefix:0000} should be valid");
        }
    }

    [Fact]
    public void MobileValidator_UnassignedPrefix_Rejected()
    {
        // 0906 is a well-formed 11-digit number but its prefix is not assigned.
        var result = MobileValidator.Instance.Validate("09061234567");
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void PostalCode_FirstDigitZero_Rejected()
    {
        var result = PostalCodeValidator.Instance.Validate("0123456789");
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void PostalCode_AllDigitsValid_AfterNormalization()
    {
        // With Persian digits that normalize to a valid first digit
        var result = PostalCodeValidator.Instance.Validate("۱۲۳۴۵۶۷۸۹۰");
        result.Success.Should().BeTrue();
    }
}

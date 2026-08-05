using System.Collections.Concurrent;
using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Algorithms;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core;

/// <summary>
/// Gate 6 — thread-safety: every validator must be safe for concurrent use.
/// The validators are stateless singletons; the differential check below
/// compares concurrent results against a single-threaded baseline for each
/// input, which catches races, shared mutable state and hidden exceptions
/// far better than a bare "no exception" assertion.
/// </summary>
public sealed class ValidatorThreadSafetyTests
{
    public sealed record ValidatorCase(string Name, IStringValidator Validator, string Valid, string[] Invalid);

    public static IEnumerable<object[]> AllCases()
    {
        yield return new object[] { new ValidatorCase("Mobile", MobileValidator.Instance, "09121234567", ["0912a456789", "00000000000", "09061234567", "0912123456"]) };
        yield return new object[] { new ValidatorCase("NationalCode", NationalCodeValidator.Instance, "0010350829", ["0000000000", "001035082A", "001035082"]) };
        yield return new object[] { new ValidatorCase("PostalCode", PostalCodeValidator.Instance, "1234567890", ["0123456789", "123456789A", "123456789"]) };
        yield return new object[] { new ValidatorCase("Telephone", TelephoneValidator.Instance, "02122345678", ["01212345678", "0211234567A", "09112345678", "0211234567"]) };
        yield return new object[] { new ValidatorCase("CardNumber", CardNumberValidator.Instance, "6037991234567893", ["6037991234567890", "603799123456789A", "0000000000000000", "603799123456789"]) };
        yield return new object[] { new ValidatorCase("Iban", IbanValidator.Instance, "IR820540102680020817909002", ["IR820540102680020817909000", "XX820540102680020817909002", "IRAAAAAAAAAAAAAAAAAAAAAAAA", "IR82054010268002081790902"]) };
        yield return new object[] { new ValidatorCase("CompanyId", CompanyIdValidator.Instance, "10380284795", ["1038028479A", "1038028479", "103802847951"]) };
        yield return new object[] { new ValidatorCase("EconomicCode", EconomicCodeValidator.Instance, "123456789019", ["12345678901A", "000000000000", "12345678901"]) };
        yield return new object[] { new ValidatorCase("Passport", PassportValidator.Instance, "P12345678", ["Z12345678", "P1234567A", "1234567A", "1234567"]) };
        yield return new object[] { new ValidatorCase("VehiclePlate", VehiclePlateValidator.Instance, "12ب34567", ["12@34567", "12ب34A67", "1AB34567", "12B3456"]) };
    }

    [Theory]
    [MemberData(nameof(AllCases))]
    public void Validate_ConcurrentReads_MatchSingleThreadedBaseline(ValidatorCase tc)
    {
        var inputs = new List<string?>
        {
            tc.Valid,
            null,
            string.Empty,
            "   ",
            ToPersianDigits(tc.Valid),
            "\u200F" + tc.Valid,
            new string('0', 5000),
        };
        inputs.AddRange(tc.Invalid);

        // Single-threaded baseline: Success + ErrorCode per input, both overloads.
        var baselineString = new (bool Success, ValidationErrorCode Code)[inputs.Count];
        var baselineSpan = new (bool Success, ValidationErrorCode Code)[inputs.Count];
        for (int i = 0; i < inputs.Count; i++)
        {
            baselineString[i] = Result(tc.Validator.Validate(inputs[i]!));
            baselineSpan[i] = inputs[i] is null ? default : Result(tc.Validator.Validate(inputs[i]!.AsSpan()));
        }

        var mismatches = new ConcurrentBag<string>();
        int repetitions = 8 * Environment.ProcessorCount;

        Parallel.For(0, repetitions, _ =>
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                var rs = Result(tc.Validator.Validate(inputs[i]!));
                if (rs != baselineString[i])
                    mismatches.Add($"[{tc.Name}] string overload mismatch at input #{i} ({Describe(inputs[i])}): got {rs}, baseline {baselineString[i]}");

                if (inputs[i] is not null)
                {
                    var rp = Result(tc.Validator.Validate(inputs[i]!.AsSpan()));
                    if (rp != baselineSpan[i])
                        mismatches.Add($"[{tc.Name}] span overload mismatch at input #{i} ({Describe(inputs[i])}): got {rp}, baseline {baselineSpan[i]}");
                }
            }
        });

        mismatches.Should().BeEmpty();
    }

    [Fact]
    public void LuhnAlgorithm_ConcurrentReads_MatchSingleThreadedBaseline()
    {
        var rng = new Random(4242);
        var corpus = Enumerable.Range(0, 2000).Select(_ => RandomDigits(rng, 16)).ToArray();
        var baseline = corpus.Select(s => LuhnAlgorithm.Validate(s)).ToArray();

        var mismatches = new ConcurrentBag<string>();
        Parallel.For(0, 8 * Environment.ProcessorCount, _ =>
        {
            for (int i = 0; i < corpus.Length; i++)
            {
                if (LuhnAlgorithm.Validate(corpus[i]) != baseline[i])
                    mismatches.Add($"Luhn mismatch at #{i}");
            }
        });

        mismatches.Should().BeEmpty();
    }

    [Fact]
    public void IbanAlgorithm_ConcurrentReads_MatchSingleThreadedBaseline()
    {
        var rng = new Random(4243);
        var corpus = Enumerable.Range(0, 2000).Select(_ => "IR" + RandomDigits(rng, 24)).ToArray();
        var baseline = corpus.Select(s => IbanAlgorithm.Validate(s)).ToArray();

        var mismatches = new ConcurrentBag<string>();
        Parallel.For(0, 8 * Environment.ProcessorCount, _ =>
        {
            for (int i = 0; i < corpus.Length; i++)
            {
                if (IbanAlgorithm.Validate(corpus[i]) != baseline[i])
                    mismatches.Add($"Iban mismatch at #{i}");
            }
        });

        mismatches.Should().BeEmpty();
    }

    [Fact]
    public void NationalCodeAlgorithm_ConcurrentReads_MatchSingleThreadedBaseline()
    {
        var rng = new Random(4244);
        var corpus = Enumerable.Range(0, 2000).Select(_ => RandomDigits(rng, 10)).ToArray();
        var baseline = corpus.Select(s => NationalCodeAlgorithm.Validate(s)).ToArray();

        var mismatches = new ConcurrentBag<string>();
        Parallel.For(0, 8 * Environment.ProcessorCount, _ =>
        {
            for (int i = 0; i < corpus.Length; i++)
            {
                if (NationalCodeAlgorithm.Validate(corpus[i]) != baseline[i])
                    mismatches.Add($"NationalCode mismatch at #{i}");
            }
        });

        mismatches.Should().BeEmpty();
    }

    private static (bool Success, ValidationErrorCode Code) Result(ValidationResult r) => (r.Success, r.ErrorCode);

    private static string ToPersianDigits(string s)
    {
        var chars = s.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] is >= '0' and <= '9')
                chars[i] = (char)('۰' + (chars[i] - '0'));
        }

        return new string(chars);
    }

    private static string RandomDigits(Random rng, int length)
    {
        var chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = (char)('0' + rng.Next(10));
        return new string(chars);
    }

    private static string Describe(string? s) => s is null ? "<null>" : $"\"{s[..Math.Min(s.Length, 20)]}\"";
}

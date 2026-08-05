using FluentAssertions;
using IranValidator.Core;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core;

/// <summary>
/// Gate 6 — deterministic fuzz tests (seeded PRNG, no external randomness, so
/// results are reproducible in CI). Properties under test for every validator:
///   1. Validate never throws on arbitrary input (garbage, unicode, RTL marks,
///      zero-width chars, mutated near-valid samples, very long strings);
///   2. The string and span overloads always agree (Success + ErrorCode).
/// Plus normalizer properties: never throws, idempotent, and the documented
/// zero-allocation fast path returns the original reference for clean ASCII.
/// </summary>
public sealed class ValidatorFuzzTests
{
    private const int Seed = 12345;

    private static readonly string[] ValidSamples =
    [
        "09121234567",          // Mobile
        "0010350829",           // NationalCode
        "1234567890",           // PostalCode
        "02122345678",          // Telephone
        "6037991234567893",     // CardNumber
        "IR820540102680020817909002", // Iban
        "10380284795",          // CompanyId
        "123456789019",         // EconomicCode
        "P12345678",            // Passport (new format)
        "12ب34567",             // VehiclePlate
    ];

    private static readonly (string Name, IStringValidator Validator)[] AllValidators =
    [
        ("Mobile", MobileValidator.Instance),
        ("NationalCode", NationalCodeValidator.Instance),
        ("PostalCode", PostalCodeValidator.Instance),
        ("Telephone", TelephoneValidator.Instance),
        ("CardNumber", CardNumberValidator.Instance),
        ("Iban", IbanValidator.Instance),
        ("CompanyId", CompanyIdValidator.Instance),
        ("EconomicCode", EconomicCodeValidator.Instance),
        ("Passport", PassportValidator.Instance),
        ("VehiclePlate", VehiclePlateValidator.Instance),
    ];

    private static readonly char[] UnicodePool =
    [
        // Latin
        '0', '1', '9', 'A', 'I', 'R', 'Z', 'a', 'z', 'B',
        // Persian digits + letters
        '۰', '۹', 'پ', 'چ', 'ژ', 'گ', 'ک', 'ی', 'ب', 'ج', 'د', 'س', 'ش', 'ص', 'ط', 'ع', 'ق', 'ل', 'م', 'ن', 'و', 'ه', 'ا',
        // Arabic digits + letters
        '٠', '٩', 'ح', 'خ',
        // Direction marks, zero-width, bidi overrides
        '\u200C', '\u200D', '\u200E', '\u200F', '\u202A', '\u202B', '\u202C', '\u202D', '\u202E',
        // Formatting / separators
        ' ', '\t', '-', '_', '.', '+', '/',
    ];

    /// <summary>Builds the deterministic fuzz corpus once per test run.</summary>
    private static List<string> BuildCorpus()
    {
        var rng = new Random(Seed);
        var corpus = new List<string>();

        // 1. Random digit strings (lengths 0..40)
        for (int i = 0; i < 2500; i++)
            corpus.Add(RandomString(rng, rng.Next(0, 41), "0123456789"));

        // 2. Random ASCII printable + control (lengths 0..30)
        for (int i = 0; i < 1500; i++)
            corpus.Add(RandomString(rng, rng.Next(0, 31), "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~\t\n"));

        // 3. Random Unicode soup (Persian/Arabic digits, letters, marks, bidi)
        for (int i = 0; i < 2000; i++)
            corpus.Add(RandomString(rng, rng.Next(0, 41), new string(UnicodePool)));

        // 4. Mutated valid samples (200 mutations per sample)
        for (int i = 0; i < ValidSamples.Length * 200; i++)
            corpus.Add(Mutate(rng, ValidSamples[i % ValidSamples.Length]));

        // 5. Near-valid: truncations and extensions of valid samples
        foreach (string sample in ValidSamples)
        {
            corpus.Add(sample[..^1]);
            corpus.Add(sample + "0");
            corpus.Add(sample[..^1] + "9");
            corpus.Add("  " + sample + "  ");
            corpus.Add(sample + sample[..4]);
        }

        // 6. All-same-character strings (digit and non-digit)
        foreach (char c in new[] { '0', '1', '9', '۰', '٩', ' ', '-' })
        {
            for (int n = 0; n <= 40; n++)
                corpus.Add(new string(c, n));
        }

        // 7. Marks-only and mark-heavy strings
        for (int i = 0; i < 500; i++)
            corpus.Add(RandomString(rng, rng.Next(1, 21), "\u200C\u200D\u200E\u200F\u202A\u202B\u202C\u202D\u202E"));

        return corpus;
    }

    [Theory]
    [MemberData(nameof(ValidatorCases))]
    public void Validate_NeverThrows_AndOverloadsAgree((string Name, IStringValidator Validator) v)
    {
        var failures = new List<string>();
        foreach (string input in BuildCorpus())
        {
            ValidationResult rs = v.Validator.Validate(input);
            ValidationResult rp = v.Validator.Validate(input.AsSpan());

            if (rs.Success != rp.Success || rs.ErrorCode != rp.ErrorCode)
                failures.Add($"[{v.Name}] overload mismatch on \"{Escape(input)}\": string=({rs.Success},{rs.ErrorCode}) span=({rp.Success},{rp.ErrorCode})");
        }

        failures.Should().BeEmpty();
    }

    [Fact]
    public void Normalize_NeverThrows_IsIdempotent_AndFastPathReturnsOriginal()
    {
        var normalizer = new CompositeNormalizer();
        var failures = new List<string>();

        foreach (string input in BuildCorpus())
        {
            string once = normalizer.Normalize(input.AsSpan());
            string twice = normalizer.Normalize(once.AsSpan());

            if (!string.Equals(once, twice, StringComparison.Ordinal))
                failures.Add($"idempotence violated: \"{Escape(input)}\" -> \"{Escape(once)}\" -> \"{Escape(twice)}\"");

            // The two-argument overload must match the span-only overload's content.
            string withOriginal = normalizer.Normalize(input.AsSpan(), input);
            if (!string.Equals(once, withOriginal, StringComparison.Ordinal))
                failures.Add($"original-aware overload differs: \"{Escape(input)}\"");

            // Documented zero-alloc fast path: clean ASCII input (0x21..0x7E,
            // no whitespace, no dash) returns the original string reference.
            if (!CompositeNormalizer.NeedsNormalization(input) && !ReferenceEquals(input, withOriginal))
                failures.Add($"fast path not taken for clean input \"{Escape(input)}\"");
        }

        failures.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(ValidatorCases))]
    public void CanonicalSamples_RemainValid((string Name, IStringValidator Validator) v)
    {
        // Sanity anchor: the fuzz corpus contains valid samples, so the
        // overload-agreement property above exercises success paths too.
        v.Validator.Validate(ValidSampleFor(v.Name)).Success.Should().BeTrue($"{v.Name} canonical sample must stay valid");
    }

    public static IEnumerable<object[]> ValidatorCases() => AllValidators.Select(v => new object[] { v });

    private static string ValidSampleFor(string validatorName) => validatorName switch
    {
        "Mobile" => "09121234567",
        "NationalCode" => "0010350829",
        "PostalCode" => "1234567890",
        "Telephone" => "02122345678",
        "CardNumber" => "6037991234567893",
        "Iban" => "IR820540102680020817909002",
        "CompanyId" => "10380284795",
        "EconomicCode" => "123456789019",
        "Passport" => "P12345678",
        "VehiclePlate" => "12ب34567",
        _ => throw new ArgumentOutOfRangeException(nameof(validatorName)),
    };

    private static string RandomString(Random rng, int length, string alphabet)
    {
        var chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = alphabet[rng.Next(alphabet.Length)];
        return new string(chars);
    }

    private static string Mutate(Random rng, string s)
    {
        if (s.Length == 0)
            return s;

        switch (rng.Next(5))
        {
            case 0: // substitute with a random digit
                {
                    var chars = s.ToCharArray();
                    chars[rng.Next(chars.Length)] = (char)('0' + rng.Next(10));
                    return new string(chars);
                }
            case 1: // substitute with a pool character (digits, letters, marks)
                {
                    var chars = s.ToCharArray();
                    chars[rng.Next(chars.Length)] = UnicodePool[rng.Next(UnicodePool.Length)];
                    return new string(chars);
                }
            case 2: // delete one char
                return s.Remove(rng.Next(s.Length), 1);
            case 3: // insert one char
                return s.Insert(rng.Next(s.Length + 1), ((char)('0' + rng.Next(10))).ToString());
            default: // swap two adjacent chars
                {
                    var chars = s.ToCharArray();
                    int i = rng.Next(chars.Length - 1);
                    (chars[i], chars[i + 1]) = (chars[i + 1], chars[i]);
                    return new string(chars);
                }
        }
    }

    private static string Escape(string s) => s
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\t", "\\t")
        .Replace("\n", "\\n");
}

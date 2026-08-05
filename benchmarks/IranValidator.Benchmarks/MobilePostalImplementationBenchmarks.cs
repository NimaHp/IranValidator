using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using IranValidator.Core.Validators;

namespace IranValidator.Benchmarks;

/// <summary>
/// Internal implementation exploration: hand-rolled span validation vs
/// compiled Regex vs GeneratedRegex for the two simplest validators
/// (mobile, postal code). On .NET 10 the source-generated regex engine is
/// fast enough that it can beat hand-written loops — this benchmark decides
/// empirically whether the span implementations are still the right choice.
/// Inputs are pre-normalized ASCII (the common case), so regexes are not
/// penalized for missing Persian-digit normalization.
/// </summary>
[MemoryDiagnoser]
public partial class MobilePostalImplementationBenchmarks
{
    private readonly string _mobile = "09121234567";
    private readonly string _postalCode = "1145687654";

    private static readonly Regex MobileRegex = new(
        @"^09[1-9][0-9]{8}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex PostalRegex = new(
        @"^[1-9][0-9]{9}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    // === Mobile ===

    [Benchmark]
    public bool MobileSpanValidator()
        => MobileValidator.Instance.Validate(_mobile).Success;

    [Benchmark]
    public bool MobileRegexCompiled()
        => MobileRegex.IsMatch(_mobile);

    [Benchmark]
    public bool MobileGeneratedRegex()
        => IsValidMobileGenerated(_mobile);

    [GeneratedRegex(@"^09[1-9][0-9]{8}$", RegexOptions.CultureInvariant)]
    private static partial Regex MobilePattern();

    private static bool IsValidMobileGenerated(string value)
        => MobilePattern().IsMatch(value);

    // === Postal code ===

    [Benchmark]
    public bool PostalSpanValidator()
        => PostalCodeValidator.Instance.Validate(_postalCode).Success;

    [Benchmark]
    public bool PostalRegexCompiled()
        => PostalRegex.IsMatch(_postalCode);

    [Benchmark]
    public bool PostalGeneratedRegex()
        => IsValidPostalGenerated(_postalCode);

    [GeneratedRegex(@"^[1-9][0-9]{9}$", RegexOptions.CultureInvariant)]
    private static partial Regex PostalPattern();

    private static bool IsValidPostalGenerated(string value)
        => PostalPattern().IsMatch(value);
}

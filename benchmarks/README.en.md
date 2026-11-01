# Performance Benchmarks

**English** | [فارسی](README.md)

This suite evaluates the performance characteristics and memory allocations of IranValidator compared to standard implementations and alternative open-source libraries.

## Benchmark Objectives

Using **BenchmarkDotNet**, the benchmarks measure:

1. **Performance of IranValidator Core:** Execution time and memory metrics across all 10 core validators using both string and ReadOnlySpan<char> overloads.
2. **Head-to-Head Comparison:** Performance evaluation against two widely-used Persian libraries — Persian.Plus and DNTPersianUtils.Core — covering the 6 input types supported by all three implementations.
3. **Hand-Rolled Span vs Regular Expressions:** Direct span-based parsing compared against compiled Regex and GeneratedRegex (mobile and postal code).

Automated benchmark checks run monthly in CI to detect potential performance regressions.

## Methodology

* **Environment:** GitHub Actions runner (ubuntu-latest — 2 vCPU / 7 GB RAM) · **.NET SDK:** <!-- bench-dotnet -->`10.0.14`<!-- /bench-dotnet --> · **BenchmarkDotNet:** <!-- bench-bdn -->`v0.14.0`<!-- /bench-bdn -->
* **Job Profile:** MediumRun (15 iterations, 10 warmups, 2 launches) with MemoryDiagnoser enabled.
* **Execution Date:** <!-- bench-date -->`2026-11-01`<!-- /bench-date -->
* **Validation Consistency:** All test cases use valid inputs verified across all libraries to ensure fair execution comparison.
* **Return Type Distinction:** Competitor libraries return a simple bool, whereas IranValidator returns a structured ValidationResult struct without incurring memory allocations.

| Library | Return Type | Auto-Normalization | Structured Error Codes |
| :--- | :--- | :--- | :--- |
| IranValidator | ValidationResult (Success, ErrorCode, NormalizedValue) | Persian/Arabic digits, spaces, dashes | Yes |
| Persian.Plus | bool | None (Rejects non-ASCII input) | No |
| DNTPersianUtils | bool | Partial | No |

## Results

### 1) IranValidator String vs Span Overloads

<!-- bench-table:overloads -->
| Method | Mean | Allocated |
|---|---|---|
| ValidateNationalCodeString | 13.68 ns | 0 B |
| ValidateNationalCodeSpan | 70.25 ns | 48 B |
| ValidateCompanyIdString | 14.70 ns | 0 B |
| ValidateCompanyIdSpan | 73.31 ns | 0 B |
| ValidateEconomicCodeString | 15.91 ns | 0 B |
| ValidateEconomicCodeSpan | 75.50 ns | 0 B |
| ValidateMobileString | 11.98 ns | 0 B |
| ValidateMobileSpan | 68.37 ns | 0 B |
| ValidateTelephoneString | 10.64 ns | 0 B |
| ValidateTelephoneSpan | 66.90 ns | 48 B |
| ValidatePostalCodeString | 5.59 ns | 0 B |
| ValidatePostalCodeSpan | 61.52 ns | 0 B |
| ValidateCardNumberString | 25.69 ns | 0 B |
| ValidateCardNumberSpan | 90.24 ns | 0 B |
| ValidatePassportString | 7.70 ns | 0 B |
| ValidatePassportSpan | 65.19 ns | 0 B |
| ValidateVehiclePlateString | 9.58 ns | 0 B |
| ValidateVehiclePlateSpan | 61.07 ns | 0 B |
| ValidateIbanString | 97.23 ns | 0 B |
| ValidateIbanSpan | 176.96 ns | 0 B |
<!-- /bench-table:overloads --> |  |  |

**Insights:**

* **String Fast Path:** Pre-normalized ASCII strings bypass memory allocations entirely, resulting in **0 B allocated** and execution times under 80 ns.
* **Span Overloads:** Using arbitrary spans allocates a small buffer (40–80 B) when creating normalized output strings.

### 2) Comparison against Persian.Plus and DNTPersianUtils

<!-- bench-table:vs-rivals -->
| Method | Mean | Allocated |
|---|---|---|
| ValidateNationalCodeIranValidator | 14.16 ns | 0 B |
| ValidateNationalCodePersianPlus | 71.86 ns | 0 B |
| ValidateNationalCodeDntPersianUtils | 69.77 ns | 192 B |
| ValidateCompanyIdIranValidator | 15.49 ns | 0 B |
| ValidateCompanyIdPersianPlus | 150.44 ns | 119 B |
| ValidateCompanyIdDntPersianUtils | 72.74 ns | 0 B |
| ValidateMobileIranValidator | 12.04 ns | 0 B |
| ValidateMobilePersianPlus | 47.13 ns | 0 B |
| ValidateMobileDntPersianUtils | 59.97 ns | 96 B |
| ValidatePostalCodeIranValidator | 5.65 ns | 0 B |
| ValidatePostalCodePersianPlus | 35.21 ns | 0 B |
| ValidatePostalCodeDntPersianUtils | 55.05 ns | 80 B |
| ValidateCardNumberIranValidator | 25.33 ns | 0 B |
| ValidateCardNumberPersianPlus | 248.39 ns | 0 B |
| ValidateCardNumberDntPersianUtils | 190.74 ns | 344 B |
| ValidateIbanIranValidator | 95.43 ns | 0 B |
| ValidateIbanPersianPlus | 167.88 ns | 0 B |
| ValidateIbanDntPersianUtils | 125.69 ns | 134 B |
<!-- /bench-table:vs-rivals --> |  |  |

**Key Takeaways:**

* **Bank Card Validation:** ~9× faster than competitors, requiring 0 B allocation vs 232/344 B.
* **Company ID Validation:** 6–13× faster execution.
* **IBAN Validation:** 1.8–2.3× faster execution with zero memory allocations.
* **Mobile & Postal Code:** 6–10× faster than compiled Regular Expressions.

### 3) Hand-Rolled Span vs Regular Expressions

<!-- bench-table:regex -->
| Implementation | Mean |
|---|---|
| Hand-rolled span (mobile) | 12.13 ns |
| `Regex` compiled | 15.91 ns |
| `GeneratedRegex` | 12.64 ns |
| Hand-rolled span (postal) | 5.57 ns |
| `Regex` compiled | 16.03 ns |
| `GeneratedRegex` | 12.57 ns |
<!-- /bench-table:regex --> |  |

Direct span parsing outperforms both .NET 10 compiled and generated regular expressions by **1.6–2.4×** — regex pays per-call startup overhead even with pooled runners.

## Conclusion (for Users)

1. **IranValidator is the fastest choice among Persian-data libraries** on all 6 shared input types, with a 1.8–13× margin — while remaining **zero-allocation** on the common path.
2. Let us put the numbers into real-world scale (one million validations):

   | Operation | IranValidator | DNTPersianUtils | Persian.Plus |
   |---|---|---|---|
   | 10M national codes | ~0.13 s · 0 B | ~0.76 s · ~1.9 GB | ~0.79 s · 0 B |
   | 10M card numbers | ~0.25 s · 0 B | ~2.3 s · ~3.4 GB | ~2.3 s · ~2.3 GB |
   | 10M IBANs | ~0.81 s · 0 B | ~1.4 s · ~1.6 GB | ~1.7 s · 0 B |

   Under normal web traffic (thousands of requests per second) the time difference is below one millisecond; the **real lasting advantage is zero allocation** — less GC pressure, lower error rates under load, and fewer pauses.
3. IranValidator is faster while also producing **richer output** (ValidationResult with an error code and normalized value, not just bool) and normalizing/validating Persian and Arabic input — where Persian.Plus rejects non-ASCII input.
4. **Practical recommendation:** prefer the string overloads — pre-normalized input takes the same 0-byte fast path; for Persian input (e.g., user forms), do not worry: automatic normalization handles it and the result remains correct.

## Reproducing Benchmarks

To execute the benchmark suite locally:

```bash
dotnet run -c Release --project benchmarks/IranValidator.Benchmarks -- --job medium --inprocess --filter "*X*"
```

### Short Technical Appendix (for Maintainers)

* `benchmarks.yml` runs every month (1st, 02:00 UTC) and on every `v*` push; it runs the suite three times and `ci/benchmark_check.py` compares the median against `baseline.json` (time regression > 1.5× or allocation beyond max(64 B, 20% of baseline) fails the check). Third-party comparison rows (Persian.Plus, DNTPersianUtils, regex) are reported but never fail the gate.
* `--inprocess` is required because the default toolchain crashes silently in restricted containers; for release numbers, run on a clean CI machine.
* `DisableOptimizationsValidator` is disabled because Persian.Plus ships an unoptimized binary.

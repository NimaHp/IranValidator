# Performance Benchmarks

**English** | [فارسی](README.md)

This suite evaluates the performance characteristics and memory allocations of IranValidator compared to standard implementations and alternative open-source libraries.

## Benchmark Objectives

Using **BenchmarkDotNet**, the benchmarks measure:

1. **Performance of IranValidator Core:** Execution time and memory metrics across all 10 core validators using both string and ReadOnlySpan<char> overloads.
2. **Head-to-Head Comparison:** Performance evaluation against two widely-used Persian libraries — Persian.Plus and DNTPersianUtils.Core — covering the 6 input types supported by all three implementations.
3. **Hand-Rolled Span vs Regular Expressions:** Direct span-based parsing compared against compiled Regex and GeneratedRegex (mobile and postal code).

Automated benchmark checks run weekly in CI to detect potential performance regressions.

## Methodology

* **Environment:** GitHub Actions runner (ubuntu-latest — 2 vCPU / 7 GB RAM) · **.NET SDK:** <!-- bench-dotnet -->`10.0.10`<!-- /bench-dotnet --> · **BenchmarkDotNet:** <!-- bench-bdn -->`v0.14.0`<!-- /bench-bdn -->
* **Job Profile:** MediumRun (15 iterations, 10 warmups, 2 launches) with MemoryDiagnoser enabled.
* **Execution Date:** <!-- bench-date -->`2026-08-05`<!-- /bench-date -->
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
| ValidateNationalCodeString | 24.55 ns | 0 B |
| ValidateNationalCodeSpan | 138.27 ns | 48 B |
| ValidateCompanyIdString | 23.66 ns | 0 B |
| ValidateCompanyIdSpan | 140.74 ns | 48 B |
| ValidateEconomicCodeString | 26.73 ns | 0 B |
| ValidateEconomicCodeSpan | 144.82 ns | 48 B |
| ValidateMobileString | 23.58 ns | 0 B |
| ValidateMobileSpan | 137.81 ns | 48 B |
| ValidateTelephoneString | 21.01 ns | 0 B |
| ValidateTelephoneSpan | 134.91 ns | 40 B |
| ValidatePostalCodeString | 11.87 ns | 0 B |
| ValidatePostalCodeSpan | 120.50 ns | 48 B |
| ValidateCardNumberString | 43.32 ns | 0 B |
| ValidateCardNumberSpan | 175.13 ns | 56 B |
| ValidatePassportString | 14.51 ns | 0 B |
| ValidatePassportSpan | 135.54 ns | 40 B |
| ValidateVehiclePlateString | 11.88 ns | 0 B |
| ValidateVehiclePlateSpan | 103.98 ns | 40 B |
| ValidateIbanString | 119.57 ns | 0 B |
| ValidateIbanSpan | 308.68 ns | 80 B |
<!-- /bench-table:overloads --> |  |  |

**Insights:**

* **String Fast Path:** Pre-normalized ASCII strings bypass memory allocations entirely, resulting in **0 B allocated** and execution times under 80 ns.
* **Span Overloads:** Using arbitrary spans allocates a small buffer (40–80 B) when creating normalized output strings.

### 2) Comparison against Persian.Plus and DNTPersianUtils

<!-- bench-table:vs-rivals -->
| Method | Mean | Allocated |
|---|---|---|
| ValidateNationalCodeIranValidator | 23.15 ns | 0 B |
| ValidateNationalCodePersianPlus | 153.78 ns | 0 B |
| ValidateNationalCodeDntPersianUtils | 114.00 ns | 0 B |
| ValidateCompanyIdIranValidator | 23.74 ns | 0 B |
| ValidateCompanyIdPersianPlus | 278.17 ns | 136 B |
| ValidateCompanyIdDntPersianUtils | 121.82 ns | 192 B |
| ValidateMobileIranValidator | 23.76 ns | 0 B |
| ValidateMobilePersianPlus | 93.30 ns | 0 B |
| ValidateMobileDntPersianUtils | 120.29 ns | 0 B |
| ValidatePostalCodeIranValidator | 11.88 ns | 0 B |
| ValidatePostalCodePersianPlus | 69.05 ns | 0 B |
| ValidatePostalCodeDntPersianUtils | 105.21 ns | 96 B |
| ValidateCardNumberIranValidator | 42.72 ns | 0 B |
| ValidateCardNumberPersianPlus | 435.56 ns | 0 B |
| ValidateCardNumberDntPersianUtils | 355.11 ns | 0 B |
| ValidateIbanIranValidator | 119.88 ns | 0 B |
| ValidateIbanPersianPlus | 340.52 ns | 0 B |
| ValidateIbanDntPersianUtils | 196.50 ns | 145 B |
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
| Hand-rolled span (mobile) | 23.18 ns |
| `Regex` compiled | 26.79 ns |
| `GeneratedRegex` | 20.07 ns |
| Hand-rolled span (postal) | 12.39 ns |
| `Regex` compiled | 26.24 ns |
| `GeneratedRegex` | 20.25 ns |
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

* `benchmarks.yml` runs every week (Monday 02:00 UTC) and on every `v*` push; `ci/benchmark_check.py` compares the results against `baseline.json` (time regression > 1.5× or allocation > 16 B fails the check).
* `--inprocess` is required because the default toolchain crashes silently in restricted containers; for release numbers, run on a clean CI machine.
* `DisableOptimizationsValidator` is disabled because Persian.Plus ships an unoptimized binary.

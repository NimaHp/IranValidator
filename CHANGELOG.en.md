# Changelog

**English** | [فارسی](CHANGELOG.md)

This project adheres to [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and follows [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Added

* **10 Core Validators:** National Code, Company ID, Economic Code, Bank Card Number
  (Iranian BIN + Luhn algorithm), IBAN (single-pass MOD-97 + bank code check), Mobile Number, Postal Code,
  Landline, Passport Number, and Vehicle Plate — all implemented as thread-safe singletons with
  Validate(string) and Validate(ReadOnlySpan<char>) overloads.
* **6 Packages:** Core, Localization, DataAnnotations, FluentValidation,
  AspNetCore, and MinimalApis (targeting netstandard2.0, net8.0, and net10.0).
* **Zero-Allocation Fast Path:** Optimized Normalize(span, original) using pre-scanning
  via NeedsNormalization — resulting in 0 bytes allocated for normalized inputs.
* **Automatic Normalization:** Built-in normalization for Persian/Arabic digits, zero-width non-joiners (ZWNJ), spaces, and dashes.
* **Structured Error Codes:** Added ValidationErrorCode with 10 explicit codes (InvalidLength, InvalidFormat,
  InvalidChecksum, InvalidCharacters, InvalidProvinceCode, InvalidBankCode,
  UnsupportedIssuer, ValueEmpty, InvalidAreaCode, and None).
* **Localization Infrastructure:** Persian and English resolvers supporting DI-first resolution with fallback to a static registry.
* **Benchmark Suite & Regression Guard:** Integrated BenchmarkDotNet tests with weekly performance checks in CI
  (triggers alerts on >1.5× execution time or >16B allocation increases).
* **Test Suite:** 1,099 unit tests maintaining 99.71% line and 97.85% branch coverage (enforced by a 95% CI gate).
* **Complete Bilingual Documentation:** Full documentation in Persian (primary) and English (*.en.md), along with standard community files
  (CONTRIBUTING, SECURITY, MIGRATION, and PR/Issue templates).
* **Automated Publishing:** Continuous deployment pipeline to publish packages to NuGet.org on v* release tags (release.yml).
* **Landline Phone Validator Enhancements:** Support for 31 province area codes and verified local number prefixes starting with 2–9 (0 reserved for trunk prefix, 1 for 3-digit service numbers).

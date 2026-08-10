# Changelog

**English** | [فارسی](CHANGELOG.md)

This project adheres to [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and follows [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [1.1.0] - 2026-08-10

### Added

* **Motorcycle plates:** VehiclePlateValidator now also accepts 8-digit motorcycle plates — a 3-digit province code plus a 5-digit serial, with no letter. Detection is automatic (an all-digit 8-char plate is treated as a motorcycle) and the 3-digit code is validated against the Wikipedia table (MotorcycleProvinceCodes). Unlike car plates, motorcycle plates never contain the digit 0.
* **Full plate format with the word «ایران»:** New IranWordNormalizer strips the word «ایران» so inputs like «۱۲ ب ۳۴۵ ایران ۶۷» normalize to the compact «۱۲ب۳۴۵۶۷».
* **Arabic letter normalization:** ArabicLetterNormalizer converts Arabic «ي» and «ك» to their Persian equivalents «ی» and «ک»; the plate letter and the «ایران» word are now accepted in both typings.

### Fixed

* **Vehicle plate — zero rule:** Plate digits are now 1–9 only; 0 is allowed solely as the second digit of the province code (e.g. Tehran 10/20/.../70). A 0 anywhere else is rejected with InvalidFormat, matching real plates.
* **Input length cap & safe normalization:** Values longer than 128 characters are now rejected with the new ValueTooLarge error code BEFORE normalization; the normalizer also rents pooled heap buffers for large inputs instead of unbounded stackalloc, so oversized input can no longer crash the process via StackOverflow.
* **UseIranValidation middleware:** Only ValidationException is mapped to 400; any other exception returns a generic 500 (internal details are never leaked) and is logged server-side.
* **Vehicle plate:** The Persian letter ز (Ministry of Defence series, listed by Wikipedia) was added to the set of valid issuance letters.

## [1.0.0] - 2026-08-05

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

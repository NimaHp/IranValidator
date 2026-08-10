# IranValidator

**The Standard Validator for Persian Data in .NET** — 1.0.0

[![Build](https://github.com/NimaHp/IranValidator/actions/workflows/ci.yml/badge.svg)](https://github.com/NimaHp/IranValidator/actions)
[![Benchmarks](https://github.com/NimaHp/IranValidator/actions/workflows/benchmarks.yml/badge.svg)](https://github.com/NimaHp/IranValidator/actions)
[![License](https://img.shields.io/github/license/NimaHp/IranValidator)](LICENSE)
[![NuGet version](https://img.shields.io/nuget/v/IranValidator.Core)](https://www.nuget.org/packages/IranValidator.Core)
[![NuGet downloads](https://img.shields.io/nuget/dt/IranValidator.Core)](https://www.nuget.org/packages/IranValidator.Core)
[![Release](https://img.shields.io/github/v/release/NimaHp/IranValidator)](https://github.com/NimaHp/IranValidator/releases)
[![Coverage](https://img.shields.io/badge/coverage-99.7%25%20lines%20%2F%2097.9%25%20branches-success)](https://github.com/NimaHp/IranValidator/actions/workflows/ci.yml)
[![Dependencies](https://img.shields.io/badge/dependencies-zero-brightgreen)](src/IranValidator.Core)
[![.NET](https://img.shields.io/badge/.NET-netstandard2.0%20%7C%20net8.0%20%7C%20net10.0-512BD4)](src/IranValidator.Core)
[![Last commit](https://img.shields.io/github/last-commit/NimaHp/IranValidator)](https://github.com/NimaHp/IranValidator)
[![Contributors](https://img.shields.io/github/contributors/NimaHp/IranValidator)](https://github.com/NimaHp/IranValidator/graphs/contributors)
[![Issues](https://img.shields.io/github/issues/NimaHp/IranValidator)](https://github.com/NimaHp/IranValidator/issues)
[![Code size](https://img.shields.io/github/languages/code-size/NimaHp/IranValidator)](https://github.com/NimaHp/IranValidator)
[![Top language](https://img.shields.io/github/languages/top/NimaHp/IranValidator)](https://github.com/NimaHp/IranValidator)

[فارسی](README.md) | **English**

---

## Overview

IranValidator is a lightweight, high-performance, dependency-free validation library for Iranian identity and financial data formats in .NET. It offers structured result models, multi-language localization, and seamless integration with ASP.NET Core and popular validation frameworks.

### Supported Validators (10)

| Validator | Format | Validation Logic |
| :--- | :--- | :--- |
| National Code | 10 digits | Weighted-sum checksum algorithm |
| Company ID | 11 digits | Legal weight checksum algorithm |
| Economic Code | 12 digits | Checksum algorithm with weights [29,27,23,19,17,13,7,5,3,2,1] |
| Mobile Number | 11 digits | Prefix verification against 42 valid 09XX operator ranges |
| Postal Code | 10 digits | Official structure rule evaluation |
| Card Number | 16 digits | Luhn algorithm + Iranian bank BIN prefix check |
| IBAN | IR prefix | Single-pass MOD-97 algorithm + 3-digit bank code verification |
| Passport Number | 1 letter + 8 digits | Standard format evaluation |
| Vehicle Plate | Standard Iranian format (car & motorcycle) | Structural pattern + province code verification |
| Landline Phone | 11 digits | 0 + 2-digit province code + 8-digit local number (starting 2–9 across 31 codes) |

> ⚠️ **Scope Limitation:** This library validates input **format, structure, and checksum integrity**. It does not query central registries to confirm if an identity, account, or line is active or currently issued. For real-time existence queries, consult authoritative services (Civil Registry, Shaparak, Telecom Operators, etc.).

### Key Features

- ✅ **Span-Based Design** — no regular expressions; zero-allocation fast path (0 B) on normalized string inputs
- ✅ **Automatic Normalization** — Converts Persian/Arabic digits and strips ZWNJ, spaces, and dashes automatically (e.g., "۰۹۱۲-۱۲۳ ۴۵۶۷" evaluates as valid)
- ✅ **Structured Results** — Returns ValidationResult with Success, NormalizedValue, and ErrorCode
- ✅ **Localization Ready** — Built-in Persian and English localized messages with an extensible fallback chain
- ✅ **Multi-Target Support** — Compatible with netstandard2.0, net8.0, and net10.0
- ✅ **Zero External Dependencies** — Core library has no third-party dependencies
- ✅ **Thread-Safe Architecture** — Stateless singletons safe for concurrent use
- ✅ **Ecosystem Integration** — DataAnnotations, FluentValidation, ASP.NET Core, and Minimal APIs


## Quick Start

```csharp
using IranValidator.Core.Extensions;

// Extension Methods
bool isValidNationalCode = "0010350829".IsIranNationalCode();      // true
bool isValidMobile       = "09121234567".IsIranMobile();          // true
bool isValidPostalCode   = "1145687654".IsIranPostalCode();       // true
bool isValidIban         = "IR820540102680020817909002".IsIranIban(); // true

// Automatic Normalization Support
bool result = "۰۹۱۲-۱۲۳ ۴۵۶۷".IsIranMobile();                   // true
```

```csharp
using IranValidator.Core.Validators;

var validator = NationalCodeValidator.Instance;
var result = validator.Validate("0010350829");

if (result.Success)
{
    Console.WriteLine($"Valid input. Normalized output: {result.NormalizedValue}");
}
```

### Integration Examples

```csharp
// DataAnnotations Attributes
[NationalCode]
[IranMobile]
public string NationalCode { get; set; }

// FluentValidation Rules
public class UserValidator : AbstractValidator<User>
{
    public UserValidator() => RuleFor(x => x.NationalCode).IranNationalCode();
}

// ASP.NET Core Middleware & Filters
builder.Services.AddIranValidation();
app.UseIranValidation();
// Apply [IranValidate] on Controller Actions

// Minimal APIs Integration
builder.Services.AddIranValidator();
app.MapPost("/users", (UserModel model, IranValidatorService validator) =>
    validator.ValidateNationalCode(model.NationalCode).Success ? Results.Ok() : Results.BadRequest());
```

For complete setup instructions, see the [Integrations Guide](docs/integrations.en.md).


## Localization

Error messages are resolved dynamically in Persian or English based on request culture or thread UICulture, with fallback mechanisms built-in.

```csharp
// Registering Custom Resolvers
IranDataAnnotationsLocalization.Configure(options =>
    options.AddResolver(CultureInfo.GetCultureInfo("fa"), new MyPersianResolver()));
```

Detailed guide: [Localization Documentation](docs/localization.en.md).


## Performance Benchmarks

Measured using **BenchmarkDotNet (MediumRun)** on GitHub Actions (ubuntu-latest) running .NET <!-- bench-dotnet -->`10.0.10`<!-- /bench-dotnet --> (BenchmarkDotNet <!-- bench-bdn -->`v0.14.0`<!-- /bench-bdn -->). Lower values represent better performance.

### Execution Time (ns)

<!-- bench-table:summary-time -->
| Validator | IranValidator | Persian.Plus | DNTPersianUtils |
| :--- | :--- | :--- | :--- |
| National Code | 23.15 | 153.78 | 114.00 |
| Company ID | 23.74 | 278.17 | 121.82 |
| Mobile | 23.76 | 93.30 | 120.29 |
| Postal Code | 11.88 | 69.05 | 105.21 |
| Card Number | 42.72 | 435.56 | 355.11 |
| IBAN | 119.88 | 340.52 | 196.50 |
<!-- /bench-table:summary-time -->

### Memory Allocation (Bytes)

<!-- bench-table:summary-alloc -->
| Validator | IranValidator | Persian.Plus | DNTPersianUtils |
| :--- | :--- | :--- | :--- |
| National Code | 0 | 0 | 0 |
| Company ID | 0 | 136 | 192 |
| Mobile | 0 | 0 | 0 |
| Postal Code | 0 | 0 | 96 |
| Card Number | 0 | 0 | 0 |
| IBAN | 0 | 0 | 145 |
<!-- /bench-table:summary-alloc -->

**Key Takeaways:**

- **Zero Allocation Path:** When inputs are pre-normalized ASCII strings (the common path), execution avoids heap allocations.
- **Rich Results:** Unlike libraries returning simple booleans, IranValidator yields a detailed ValidationResult struct without memory overhead.

Benchmark reproduction steps: [Benchmark Suite Documentation](benchmarks/README.en.md).

## Available Packages

| Package | Status | Description |
| :--- | :--- | :--- |
| IranValidator.Core | ✅ | Core validation engine without third-party dependencies |
| IranValidator.DataAnnotations | ✅ | Model attributes for DataAnnotations |
| IranValidator.FluentValidation | ✅ | Extension rules for FluentValidation |
| IranValidator.AspNetCore | ✅ | DI registration, action filters, and middleware |
| IranValidator.MinimalApis | ✅ | Injectable validation service for Minimal APIs |
| IranValidator.Localization | ✅ | Multi-language localized error messages |

## Testing & Code Coverage

- **1,099 Unit Tests** passing across platforms (xUnit v3 + FluentAssertions)
- **Code Coverage:** 99.71% Line / 97.85% Branch (enforced via CI threshold of 95%)

| Module | Line | Branch | Method |
|---|---|---|---|
| Core | 99.86% | 99.26% | 99.24% |
| DataAnnotations | 100% | 100% | 100% |
| FluentValidation | 100% | 96.96% | 100% |
| AspNetCore | 100% | 78.57% | 100% |
| MinimalApis | 100% | 100% | 100% |
| Localization | 97.40% | 92.85% | 85.71% |

## CI/CD

- **ci.yml** — build + test + coverage gate (95% line threshold) on every push/PR
- **benchmarks.yml** — weekly benchmark run + baseline comparison (fail on regression) + run before Release
- **release.yml** — automatic publish to NuGet.org and GitHub Release creation on `v*` tags

## License

Distributed under the [MIT License](LICENSE).

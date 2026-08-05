# Contributing Guide

**English** | [فارسی](CONTRIBUTING.md)

Thank you for contributing to IranValidator! This guide will help you get started with setting up the project, running tests, and submitting your changes.

## Prerequisites

* .NET 10 SDK
* (Optional) Python 3 for running benchmark evaluation scripts

## Project Setup

Clone the repository and build the solution:

```bash
git clone <repository-url>
dotnet restore IranValidator.slnx
dotnet build IranValidator.slnx -c Release
```

## Running Tests

Execute the unit test suite:

```bash
dotnet test IranValidator.slnx -c Release --no-build
```

* All tests must pass cleanly. A **95% code coverage threshold** (lines, branches, and methods) is enforced in CI via Coverlet (ThresholdStat=total).
* To run performance benchmarks (using the exact CI execution flags):

```bash
dotnet run -c Release --project benchmarks/IranValidator.Benchmarks -- --job medium --inprocess --filter "*X*"
```

## Coding Conventions

* **Strict Warnings Policy:** Controlled via .editorconfig + TreatWarningsAsErrors. Builds with warnings will not be accepted.
* **Modern .NET Conventions:** Nullable context and ImplicitUsings are enabled project-wide.
* **Validator Design:** New validators must implement the **Singleton** pattern and inherit IStringValidator, exposing overloads for both string and ReadOnlySpan<char>.
* **Zero-Allocation Policy:** Avoid using Regex in validators (use direct Span<char> operations instead). Avoid object boxing and do not throw exceptions for validation failures.
* **Error Codes & Messages:** Failure modes must use ValidationErrorCode values. Messages must route through the Localization engine (DI-first with static registry fallback).
* **Performance:** Utilize the zero-allocation fast path via Normalize(span, original) and NeedsNormalization pre-scanning.

## Bilingual Documentation

Persian is the primary documentation language (files without a language suffix). English documentation files use the *.en.md suffix.

* Any documentation change must be reflected in **both language versions**.
* Do not remove English documentation files.
* Preserve the language-switching header at the top of every documentation file.

## Pull Request Checklist

Before opening a pull request, ensure the following steps are complete:

* [ ] Build and test suites pass with zero warnings
* [ ] Code coverage remains at or above 95%
* [ ] CHANGELOG.md is updated under the Unreleased section (in both languages)
* [ ] Documentation is updated across both language versions where appropriate
* [ ] Benchmarks have been executed for performance-sensitive modifications

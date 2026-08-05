# IranValidator.Core

The **high-performance, dependency-free core module** of [IranValidator](https://github.com/NimaHp/IranValidator) — the standard validator for Persian data in .NET.

## Installation

```
dotnet add package IranValidator.Core
```

## Usage

```csharp
using IranValidator.Core.Validators;

var validator = NationalCodeValidator.Instance;
ValidationResult result = validator.Validate("0010350829");

if (result.Success)
{
    Console.WriteLine($"Valid input. Normalized value: {result.NormalizedValue}");
}
```

All 10 core validators (National Code, Company ID, Economic Code, Mobile, Postal Code, Card Number, IBAN, Passport, Vehicle Plate, and Landline) follow the singleton pattern and support both string and ReadOnlySpan<char> overloads.

## License

MIT License — see the repository [LICENSE](https://github.com/NimaHp/IranValidator/blob/main/LICENSE).

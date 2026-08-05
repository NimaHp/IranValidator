# IranValidator.Localization

Multi-language message resolvers for [IranValidator](https://github.com/NimaHp/IranValidator) — Persian and English with an extensible fallback chain.

## Installation

```
dotnet add package IranValidator.Localization
```

## Usage

```csharp
using IranValidator.Localization;
using IranValidator.Localization.Resolvers;

services.AddIranLocalization(); // registers English and Persian built-in resolvers

// Customizing messages per culture:
IranDataAnnotationsLocalization.Configure(options =>
    options.AddResolver(CultureInfo.GetCultureInfo("fa"), new MyPersianResolver()));
```

## License

MIT License — see the repository [LICENSE](https://github.com/NimaHp/IranValidator/blob/main/LICENSE).

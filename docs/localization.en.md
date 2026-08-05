# Localization Architecture

**English** | [فارسی](localization.md)

IranValidator resolves user-facing validation messages through a culture-aware resolver chain. All four adapters — DataAnnotations, FluentValidation, ASP.NET Core, and Minimal APIs — share this exact mechanism, so one customization applies everywhere.

## Core Concepts

| Concept | Purpose |
| :--- | :--- |
| IValidationMessageResolver | Primary interface mapping a ValidationErrorCode, property name, and CultureInfo to message text. |
| ValidationMessageOptions | Thread-safe registry holding resolvers for individual languages and cultures. |
| ValidationErrorCode | Enum with 10 explicit codes: InvalidLength, InvalidFormat, InvalidChecksum, InvalidCharacters, InvalidProvinceCode, InvalidBankCode, UnsupportedIssuer, ValueEmpty, InvalidAreaCode, and None. |
| Static Entry Points | Facade classes such as IranDataAnnotationsLocalization, IranFluentValidationLocalization, and IranAspNetCoreLocalization — thin layers over the same shared registry. |

## Resolution Precedence

The message lookup chain follows this order:

1. **Exact Culture match** (e.g., fa-IR)
2. **Parent culture walk** (fa-IR → fa → Invariant)
3. **Culture configured in DefaultCulture**
4. **CultureInfo.InvariantCulture**
5. **Built-in English resolver** (final fallback)

GetMessage on the static entry points defaults the culture to CurrentUICulture, so per-request cultures (e.g., via ASP.NET Core RequestLocalizationMiddleware) are honored automatically.

## Built-in Resolvers

ValidationMessageOptionsExtensions.AddBuiltInResolvers() registers:

- **EnglishMessageResolver** for InvariantCulture, en, and en-US
- **PersianMessageResolver** for fa and fa-IR

All adapter registries start with these two resolvers; unknown cultures fall back to English (steps 3–5 above).

## Per-Adapter Usage

### DataAnnotations (IranValidator.DataAnnotations)

All 10 attributes (NationalCodeAttribute, IranMobileAttribute, IranIbanAttribute, …) derive from IranValidationAttribute. On failure, the attribute resolves messages in this order:

1. **DI First** — fetch IValidationMessageResolver from the validation context service provider (under ASP.NET Core MVC, request services are available, so with AddIranValidation, ModelState errors are localized automatically)
2. **Static Registry** — IranDataAnnotationsLocalization.GetMessage(...)

### FluentValidation (IranValidator.FluentValidation)

Rules like .IranNationalCode() attach a MessageBuilder that resolves the message **at validation time** (not rule-construction time) so the current request culture is always respected. The main validator is re-executed to obtain the precise ValidationErrorCode; as a result, messages are error-code-specific — e.g., an IBAN with an unknown bank code reports "Invalid bank code", not a generic failure.

### ASP.NET Core (IranValidator.AspNetCore)

AddIranValidation() registers a default IValidationMessageResolver in DI (an adapter over the DataAnnotations registry — Configure customizations also apply to the DI path). The action filter, error middleware, and invalid-model responses all take their HTTP title from IranAspNetCoreLocalization.GetTitle() ("Validation Error" / "خطای اعتبارسنجی").

### Minimal APIs (IranValidator.MinimalApis)

IranValidatorService returns the ValidationResult object; message formatting is left to the caller (no hardcoded English text inside the service).

## Customizing Messages

### Replacing Persian Messages

```csharp
IranDataAnnotationsLocalization.Configure(options =>
{
    options.AddResolver(
        CultureInfo.GetCultureInfo("fa"),
        new MyPersianResolver());      // any IValidationMessageResolver
});
```

### Registering a Brand-New Culture

```csharp
IranFluentValidationLocalization.Configure(options =>
{
    options.AddResolver(
        CultureInfo.GetCultureInfo("ar"),
        new ArabicResolver());
});
```

A custom resolver is nothing more than this:

```csharp
public sealed class CustomPersianResolver : IValidationMessageResolver
{
    public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
        => errorCode switch
        {
            ValidationErrorCode.InvalidLength   => $"The {propertyName ?? "input"} length is invalid.",
            ValidationErrorCode.InvalidChecksum => $"The {propertyName ?? "input"} failed the checksum verification.",
            _                                   => $"The {propertyName ?? "field"} is invalid.",
        };
}
```

ValidationMessageOptions.AddResolver replaces per-culture registrations; replacing a built-in resolver (e.g., fa) only affects that culture and its sub-cultures, while other cultures keep their built-in behavior. Registrations are thread-safe (ConcurrentDictionary), so lazy configuration at startup or first use is safe.

### Replacing the DI Path in ASP.NET Core

To use a custom resolver on the DI path without touching the static registry, register it yourself — your own AddSingleton<IValidationMessageResolver>(...) overrides the default registration made by AddIranValidation().

## Message Table

| ValidationErrorCode | English | فارسی |
| :--- | :--- | :--- |
| InvalidLength | {name} has an invalid length. | طول {name} نامعتبر است. |
| InvalidFormat | {name} has an invalid format. | قالب {name} نامعتبر است. |
| InvalidChecksum | {name} fails the checksum verification. | {name} در بررسی جمع کنترلی نامعتبر است. |
| InvalidCharacters | {name} contains invalid characters. | {name} حاوی کاراکترهای نامعتبر است. |
| InvalidProvinceCode | {name} contains an invalid province code. | {name} حاوی کد استان نامعتبر است. |
| InvalidBankCode | {name} contains an unknown bank code. | {name} حاوی کد بانک نامعتبر است. |
| UnsupportedIssuer | {name} is issued by an unsupported card network. | {name} متعلق به شبکهٔ بانکی پشتیبانی‌نشده است. |
| ValueEmpty | {name} cannot be empty. | {name} نمی‌تواند خالی باشد. |
| InvalidAreaCode | {name} contains an invalid area code. | {name} حاوی کد منطقهٔ نامعتبر است. |
| None | The value is valid. | مقدار معتبر است. |

> {name} is the display name: the property name in FluentValidation, DisplayName/MemberName in DataAnnotations, and "The field" when no context exists.

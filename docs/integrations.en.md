# Integration Guide

**English** | [فارسی](integrations.md)

IranValidator is distributed across six modular packages. Core contains no third-party dependencies, and all integration packages extend its functionality.

| Package | Dependencies | Target Scenario |
| :--- | :--- | :--- |
| IranValidator.Core | None | Direct validation, extension methods, high-performance paths |
| IranValidator.DataAnnotations | Core, Localization | Attributes for ASP.NET Core MVC / Model validation |
| IranValidator.FluentValidation | Core, Localization | Validation rules for FluentValidation |
| IranValidator.AspNetCore | DataAnnotations, Core, Localization | ASP.NET Core Dependency Injection, action filters, middleware |
| IranValidator.MinimalApis | Core, Localization | Injectable services for Minimal API endpoints |
| IranValidator.Localization | Core | Custom multi-language message resolvers |

## Core Library Usage

```csharp
using IranValidator.Core.Validators;

var validator = NationalCodeValidator.Instance;
ValidationResult result = validator.Validate("0010350829");
// result.Success == true
// result.NormalizedValue -> Standardized ASCII representation
// result.ErrorCode -> ValidationErrorCode.None on success
```

All 10 core validators implement the Singleton pattern (NationalCodeValidator.Instance, MobileValidator.Instance, etc.) and support both string and ReadOnlySpan<char> via IStringValidator. Null and empty values pass evaluation by design (following standard DataAnnotations conventions); use explicit presence rules when input is mandatory.

### Extension Methods

```csharp
using IranValidator.Core.Extensions;

"0010350829".IsIranNationalCode();         // true
"09121234567".IsIranMobile();              // true
"1145687654".IsIranPostalCode();           // true
"6037991234567893".IsIranCardNumber();     // true
"10380284795".IsIranCompanyId();           // true
"123456789019".IsIranEconomicCode();       // true
"P12345678".IsIranPassport();              // true
"02122345678".IsIranTelephone();           // true
"12ب34567".IsIranVehiclePlate();           // true
"IR820540102680020817909002".IsIranIban(); // true
```

Inputs containing Persian or Arabic digits, zero-width spaces, spaces, or dashes are normalized automatically before evaluation.

## DataAnnotations Integration

```csharp
public class UserModel
{
    [NationalCode]
    public string NationalCode { get; set; } = string.Empty;

    [IranMobile]
    public string Mobile { get; set; } = string.Empty;

    [IranIban]
    public string? Iban { get; set; } // Nullable fields pass validation if null
}
```

Available attributes: [NationalCode], [IranCardNumber], [IranCompanyId], [IranEconomicCode], [IranMobile], [IranPassport], [IranPostalCode], [IranTelephone], [IranVehiclePlate], and [IranIban].

## FluentValidation Integration

```csharp
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.NationalCode).IranNationalCode();
        RuleFor(x => x.Mobile).IranMobile();
        RuleFor(x => x.Iban).IranIban();
    }
}
```

Validation rules: IranNationalCode(), IranCardNumber(), IranCompanyId(), IranEconomicCode(), IranMobile(), IranPassport(), IranPostalCode(), IranTelephone(), IranVehiclePlate(), and IranIban().

## ASP.NET Core Integration

```csharp
// Program.cs
builder.Services.AddControllers();
builder.Services.AddIranValidation(); // DI message resolver + Problem Details (RFC 7807)
var app = builder.Build();
app.UseIranValidation();               // Middleware converting exceptions to 400 Problem Details
```

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpPost]
    [IranValidate] // Action filter automatically handling invalid ModelState
    public IActionResult Create(UserModel model) => Ok();
}
```

## Minimal APIs Integration

```csharp
builder.Services.AddIranValidator(); // Registers IranValidatorService as a singleton

app.MapPost("/api/users", (UserModel model, IranValidatorService validator) =>
{
    var result = validator.ValidateNationalCode(model.NationalCode);
    if (!result.Success)
    {
        return Results.ValidationProblem(
            new Dictionary<string, string[]> { ["nationalCode"] = new[] { result.ErrorCode.ToString() } },
            title: "Validation Error");
    }
    return Results.Ok();
});
```

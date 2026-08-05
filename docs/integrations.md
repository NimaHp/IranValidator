# راهنمای یکپارچه‌سازی (Integrations)

**فارسی** | [English](integrations.en.md)

کتابخانه IranValidator در قالب ۶ پکیج متمايز عرضه می‌شود. هسته اصلی (Core) فاقد هرگونه وابستگی خارجی است و پکیج‌های یکپارچه‌سازی، امکانات آن را در فریمورک‌های مختلف توسعه می‌دهند.

| پکیج | وابستگی‌ها | سناریوی استفاده |
| :--- | :--- | :--- |
| IranValidator.Core | فاقد وابستگی | اعتبارسنجی مستقیم، استفاده از متدهای توسعه و پردازش با کارایی بالا |
| IranValidator.DataAnnotations | Core, Localization | ویژگی‌های اعتبارسنجی مدل در ASP.NET Core MVC |
| IranValidator.FluentValidation | Core, Localization | قوانین اعتبارسنجی برای کتابخانه FluentValidation |
| IranValidator.AspNetCore | DataAnnotations, Core, Localization | ثبت تزریق وابستگی (DI)، فیلترهای اکشن و میدل‌ور ASP.NET Core |
| IranValidator.MinimalApis | Core, Localization | سرویس تزریق‌پذیر برای اندپوینت‌های Minimal API |
| IranValidator.Localization | Core | پیاده‌سازی رزولورهای سفارشی برای ترجمه پیام‌های خطا |

## نحوه استفاده از هسته (Core)

```csharp
using IranValidator.Core.Validators;

var validator = NationalCodeValidator.Instance;
ValidationResult result = validator.Validate("0010350829");
// result.Success == true
// result.NormalizedValue -> مقدار استانداردسازی‌شده به‌صورت ASCII
// result.ErrorCode -> ValidationErrorCode.None در صورت موفقیت
```

تمامی ۱۰ اعتبارسنج اصلی از الگوی Singleton پیروی می‌کنند (NationalCodeValidator.Instance و ...) و پشتیبانی از string و ReadOnlySpan<char> را فراهم می‌کنند. مقادیر null یا خالی طبق استاندارد DataAnnotations معتبر در نظر گرفته می‌شوند؛ در صورت اجباری بودن ورودی، بررسی وجود آن را به صورت جداگانه انجام دهید.

### متدهای توسعه (Extension Methods)

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

ورودی‌های حاوی اعداد فارسی/عربی، نیم‌فاصله، فاصله یا خط تیره پیش از اعتبارسنجی به صورت خودکار نرمال‌سازی می‌شوند.

## استفاده در DataAnnotations

```csharp
public class UserModel
{
    [NationalCode]
    public string NationalCode { get; set; } = string.Empty;

    [IranMobile]
    public string Mobile { get; set; } = string.Empty;

    [IranIban]
    public string? Iban { get; set; } // مقادیر نال‌پذیر در صورت null بودن معتبر پاس می‌شوند
}
```

ویژگی‌های موجود: [NationalCode]، [IranCardNumber]، [IranCompanyId]، [IranEconomicCode]، [IranMobile]، [IranPassport]، [IranPostalCode]، [IranTelephone]، [IranVehiclePlate] و [IranIban].

## استفاده در FluentValidation

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

قوانین موجود: IranNationalCode()، IranCardNumber()، IranCompanyId()، IranEconomicCode()، IranMobile()، IranPassport()، IranPostalCode()، IranTelephone()، IranVehiclePlate() و IranIban().

## استفاده در ASP.NET Core

```csharp
// Program.cs
builder.Services.AddControllers();
builder.Services.AddIranValidation(); // ثبت رزولورهای DI و ساختار RFC 7807
var app = builder.Build();
app.UseIranValidation();               // میدل‌ور برای تبدیل خطاها به خروجی Problem Details
```

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpPost]
    [IranValidate] // فیلتر اکشن برای کوتاه‌سازی خودکار پاسخ‌های نامعتبر
    public IActionResult Create(UserModel model) => Ok();
}
```

## استفاده در Minimal APIs

```csharp
builder.Services.AddIranValidator(); // ثبت IranValidatorService به‌صورت Singleton

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

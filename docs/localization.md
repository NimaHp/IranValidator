# معماری محلی‌سازی (Localization)

**فارسی** | [English](localization.en.md)

IranValidator پیام‌های نمایشی اعتبارسنجی را از طریق یک زنجیرهٔ حل فرهنگ‌آگاه (resolver chain) مدیریت می‌کند. هر چهار آداپتور — DataAnnotations، FluentValidation، ASP.NET Core و Minimal APIs — از همین سازوکار یکسان استفاده می‌کنند؛ یعنی یک سفارشی‌سازی، همه‌جا اعمال می‌شود.

## مفاهیم اصلی

| مفهوم | نقش |
| :--- | :--- |
| IValidationMessageResolver | اینترفیس اصلی برای نگاشت کد خطا (ValidationErrorCode)، نام ویژگی و Culture به پیام متنی. |
| ValidationMessageOptions | رجیستری Thread-Safe برای ثبت رزولورهای مرتبط با زبان‌ها و فرهنگ‌های مختلف. |
| ValidationErrorCode | شمارنده شامل ۱۰ کد خطای مشخص: InvalidLength، InvalidFormat، InvalidChecksum، InvalidCharacters، InvalidProvinceCode، InvalidBankCode، UnsupportedIssuer، ValueEmpty، InvalidAreaCode و None. |
| نقاط ورود ایستا | کلاس‌های رابط مانند IranDataAnnotationsLocalization، IranFluentValidationLocalization و IranAspNetCoreLocalization — لایهٔ نازک روی همان رجیستری مشترک. |

## زنجیره اولویت محلی‌سازی

فرآیند یافتن پیام خطا بر اساس ترتیب زیر انجام می‌شود:

1. **تطبیق دقیق Culture** (مانند fa-IR)
2. **بررسی Culture والد** (fa-IR → fa → Invariant)
3. **فرهنگ پیش‌فرض تنظیم‌شده در DefaultCulture**
4. **CultureInfo.InvariantCulture**
5. **رزولور انگلیسی داخلی** (به‌عنوان پشتیبان نهایی)

متد GetMessage در نقاط ورود ایستا، فرهنگ را پیش‌فرض روی CurrentUICulture قرار می‌دهد؛ بنابراین فرهنگ هر درخواست (مثلاً توسط RequestLocalizationMiddleware در ASP.NET Core) به‌صورت خودکار رعایت می‌شود.

## رزولورهای داخلی

ValidationMessageOptionsExtensions.AddBuiltInResolvers() این موارد را ثبت می‌کند:

- **EnglishMessageResolver** برای InvariantCulture، en و en-US
- **PersianMessageResolver** برای fa و fa-IR

رجیستری همهٔ آداپتورها با همین دو رزولور آغاز می‌شود؛ فرهنگ‌های ناشناخته به انگلیسی برمی‌گردند (مراحل ۳ تا ۵ بالا).

## نحوهٔ استفاده در هر آداپتور

### DataAnnotations (IranValidator.DataAnnotations)

هر ۱۰ ویژگی (NationalCodeAttribute، IranMobileAttribute، IranIbanAttribute و …) از IranValidationAttribute ارث می‌برند. هنگام خطا، ویژگی پیام را با این اولویت حل می‌کند:

1. **DI اول** — گرفتن IValidationMessageResolver از service provider کانتکست اعتبارسنجی (در ASP.NET Core MVC سرویس‌های درخواست در دسترس هستند؛ بنابراین با AddIranValidation خطاهای ModelState به‌صورت خودکار محلی‌سازی می‌شوند)
2. **رجیستری ایستا** — IranDataAnnotationsLocalization.GetMessage(...)

### FluentValidation (IranValidator.FluentValidation)

قوانینی مثل .IranNationalCode() یک MessageBuilder تنظیم می‌کنند که پیام را **هنگام اعتبارسنجی** (نه هنگام ساخت قانون) حل می‌کند تا فرهنگ جاری درخواست همیشه رعایت شود. ولیدیتور اصلی دوباره اجرا می‌شود تا ValidationErrorCode دقیق به دست آید؛ نتیجه این است که پیام‌ها مخصوص کد خطا هستند — مثلاً شبا با کد بانک ناشناخته، گزارش «کد بانک نامعتبر» می‌دهد، نه یک خطای عمومی.

### ASP.NET Core (IranValidator.AspNetCore)

AddIranValidation() رزولور پیش‌فرض IValidationMessageResolver را در DI ثبت می‌کند (آداپتوری روی رجیستری DataAnnotations — سفارشی‌سازی‌های Configure روی مسیر DI هم اعمال می‌شوند). فیلتر اکشن، middleware خطا و پاسخ مدل نامعتبر، هر سه عنوان HTTP خود را از IranAspNetCoreLocalization.GetTitle() می‌گیرند («Validation Error» / «خطای اعتبارسنجی»).

### Minimal APIs (IranValidator.MinimalApis)

IranValidatorService آبجکت ValidationResult برمی‌گرداند؛ قالب‌بندی پیام با خودِ فراخوان است (هیچ متن انگلیسی هاردکدی داخل سرویس نیست).

## سفارشی‌سازی پیام‌ها

### جایگزینی پیام‌های فارسی

```csharp
IranDataAnnotationsLocalization.Configure(options =>
{
    options.AddResolver(
        CultureInfo.GetCultureInfo("fa"),
        new MyPersianResolver());      // هر IValidationMessageResolver
});
```

### ثبت یک فرهنگ کاملاً جدید

```csharp
IranFluentValidationLocalization.Configure(options =>
{
    options.AddResolver(
        CultureInfo.GetCultureInfo("ar"),
        new ArabicResolver());
});
```

یک رزولور سفارشی فقط همین است:

```csharp
public sealed class CustomPersianResolver : IValidationMessageResolver
{
    public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
        => errorCode switch
        {
            ValidationErrorCode.InvalidLength   => $"طول {propertyName ?? "ورودی"} معتبر نیست.",
            ValidationErrorCode.InvalidChecksum => $"شناسه {propertyName ?? "ورودی"} با ساختار منطقی مطابقت ندارد.",
            _                                   => $"ورودی {propertyName ?? "مورد نظر"} نامعتبر است.",
        };
}
```

ValidationMessageOptions.AddResolver به‌ازای هر فرهنگ جایگزین می‌کند؛ پس جایگزینی رزولور داخلی (مثلاً fa) فقط همان فرهنگ و زیرفرهنگ‌هایش را تحت تأثیر قرار می‌دهد و بقیهٔ فرهنگ‌ها رفتار داخلی خود را حفظ می‌کنند. ثبت‌ها thread-safe هستند (ConcurrentDictionary)؛ بنابراین پیکربندی lazy در شروع یا اولین استفاده امن است.

### جایگزینی مسیر DI در ASP.NET Core

برای استفاده از رزولور سفارشی در مسیر DI بدون دست زدن به رجیستری ایستا، کافی است خودتان آن را ثبت کنید — ثبت singleton شخصی شما با AddSingleton<IValidationMessageResolver>(...) جایگزین ثبت پیش‌فرض AddIranValidation() می‌شود.

## جدول پیام‌ها

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

> {name} نام نمایشی است: در FluentValidation نام ویژگی، در DataAnnotations DisplayName/MemberName، و وقتی کانتکستی وجود ندارد «The field».

# IranValidator

**اعتبارسنج استاندارد داده‌های فارسی در `.NET`** — نسخه `1.1.0`

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

**فارسی** | [English](README.en.md)

---

## درباره پروژه

مجموعه‌ای سبک، فوق‌العاده سریع و بدون وابستگی خارجی برای اعتبارسنجی داده‌های هویت ایرانی در `.NET` — همراه با خروجی‌های ساختاریافته، پشتیبانی از چندزبانگی (Localization) و یکپارچگی کامل با ASP.NET Core و فریمورک‌های رایج.

### اعتبارسنج‌ها (۱۰ مورد)

<table dir="rtl">
<thead>
<tr>
<th>اعتبارسنج</th>
<th>نوع ورودی</th>
<th>الگوریتم اعتبارسنجی</th>
</tr>
</thead>

<tbody>
<tr>
<td>کد ملی</td>
<td>۱۰ رقم</td>
<td>الگوریتم <span dir="ltr">Checksum</span> وزنی</td>
</tr>

<tr>
<td>شناسه ملی شرکت</td>
<td>۱۱ رقم</td>
<td>الگوریتم <span dir="ltr">Checksum</span> با وزن‌های قانونی اشخاص حقوقی</td>
</tr>

<tr>
<td>کد اقتصادی</td>
<td>۱۲ رقم</td>
<td>
الگوریتم <span dir="ltr">Checksum</span> با ضرایب
<span dir="ltr">[29,27,23,19,17,13,7,5,3,2,1]</span>
</td>
</tr>

<tr>
<td>شماره موبایل</td>
<td>۱۱ رقم</td>
<td>بررسی پیش‌شماره‌های فعال <span dir="ltr">09XX</span> (جدول ۴۲ دامنه‌ای)</td>
</tr>

<tr>
<td>کد پستی</td>
<td>۱۰ رقم</td>
<td>ساختار استاندارد پست ایران</td>
</tr>

<tr>
<td>شماره کارت بانکی</td>
<td>۱۶ رقم</td>
<td>
الگوریتم <span dir="ltr">Luhn</span> + اعتبارسنجی پیش‌شماره <span dir="ltr">BIN</span> بانک‌ها
</td>
</tr>

<tr>
<td>شماره شبا (<span dir="ltr">IBAN</span>)</td>
<td>پیشوند <span dir="ltr">IR</span></td>
<td>پویش تک‌گذر <span dir="ltr">MOD-97</span> + اعتبارسنجی کد ۳ رقمی بانک</td>
</tr>

<tr>
<td>شماره پاسپورت</td>
<td>۱ حرف + ۸ رقم</td>
<td>فرمت رسمی گذرنامه</td>
</tr>

<tr>
<td>پلاک خودرو</td>
<td>فرمت استاندارد</td>
<td>ساختار رسمی پلاک + کد استان</td>
</tr>

<tr>
<td>تلفن ثابت</td>
<td>۱۱ رقم</td>
<td>۰ + کد استان ۲رقمی + شماره محلی ۸رقمی (شروع ۲–۹، ۳۱ کد استان)</td>
</tr>
</tbody>
</table>

> ⚠️ **توضیح مهم:** این کتابخانه صرفاً **ساختار، فرمت و صحت ریاضی Checksum** ورودی را بررسی می‌کند و عدم مغایرت ساختاری به معنای استعلام فعال بودن یا وجود داشتن واقعی آن رکورد در سامانه‌های ثبت‌احوال، بانک‌ها یا اپراتورها نیست.

### ویژگی‌های اصلی

- ✅ **طراحی مبتنی بر Span** — بدون Regex؛ مسیر سریعِ بدون تخصیص (۰ بایت) روی ورودی‌های نرمال‌شدهٔ رشت‌ای
- ✅ **نرمال‌سازی خودکار** — تبدیل اعداد فارسی/عربی و حذف فاصله و خط تیره (ورودی `"0912-123 4567"` معتبر است)
- ✅ **خروجی ساختاریافته** — بازگرداندن ValidationResult شامل Success، NormalizedValue و ErrorCode
- ✅ **پشتیبانی از چندزبانگی** — پیام‌های خطا به زبان‌های فارسی و انگلیسی با امکان توسعه
- ✅ **پشتیبانی از پلتفرم‌های متنوع** — قابل استفاده در `netstandard2.0`، `net8.0` و `net10.0`
- ✅ **بدون وابستگی خارجی** — هسته اصلی پروژه وابسته به هیچ پکیج جانبی نیست
- ✅ **ایمن در محیط‌های چندنخی** — ساختار Stateless و Thread-Safe
- ✅ **یکپارچگی آسان** — پشتیبانی از DataAnnotations، FluentValidation، ASP.NET Core و Minimal APIs

## شروع سریع

```csharp
using IranValidator.Core.Extensions;

// استفاده از Extension Methodها
bool isValidNationalCode = "0010350829".IsIranNationalCode();      // true
bool isValidMobile       = "09121234567".IsIranMobile();          // true
bool isValidPostalCode   = "1145687654".IsIranPostalCode();       // true
bool isValidIban         = "IR820540102680020817909002".IsIranIban(); // true

// نرمال‌سازی خودکار ورودی
bool result = "۰۹۱۲-۱۲۳ ۴۵۶۷".IsIranMobile();                   // true
```

```csharp
using IranValidator.Core.Validators;

var validator = NationalCodeValidator.Instance;
var result = validator.Validate("0010350829");

if (result.Success)
{
    Console.WriteLine($"✅ معتبر — مقدار نرمال‌شده: {result.NormalizedValue}");
}
```

### یکپارچگی با فریمورک‌ها

```csharp
// DataAnnotations
[NationalCode]
[IranMobile]
public string NationalCode { get; set; }

// FluentValidation
public class UserValidator : AbstractValidator<User>
{
    public UserValidator() => RuleFor(x => x.NationalCode).IranNationalCode();
}

// ASP.NET Core
builder.Services.AddIranValidation();
app.UseIranValidation();
// استفاده از [IranValidate] روی اکشن‌ها

// Minimal APIs
builder.Services.AddIranValidator();
app.MapPost("/users", (UserModel model, IranValidatorService validator) =>
    validator.ValidateNationalCode(model.NationalCode).Success ? Results.Ok() : Results.BadRequest());
```

راهنمای کامل: [مستندات یکپارچه‌سازی](docs/integrations.md)

## محلی‌سازی (Localization)

پیام‌های خطا بر اساس فرهنگ درخواست (Request Culture) یا UI Culture برنامه به صورت خودکار به زبان فارسی یا انگلیسی ترجمه می‌شوند.

```csharp
// Registering Custom Resolvers
IranDataAnnotationsLocalization.Configure(options =>
    options.AddResolver(CultureInfo.GetCultureInfo("fa"), new MyPersianResolver()));
```

مستندات کامل: [راهنمای محلی‌سازی](docs/localization.md)

## بنچمارک و کارایی

اندازه‌گیری‌شده توسط **BenchmarkDotNet (MediumRun)** روی محیط GitHub Actions (ubuntu-latest) و دات‌نت <!-- bench-dotnet -->`10.0.10`<!-- /bench-dotnet --> (نسخهٔ BenchmarkDotNet: <!-- bench-bdn -->`v0.14.0`<!-- /bench-bdn -->). مقادیر کمتر نشان‌دهنده عملکرد بهتر هستند.

### زمان اجرا (نانوثانیه)

<!-- bench-table:summary-time -->
<table dir="rtl">
<thead>
<tr><th>اعتبارسنج</th><th>IranValidator</th><th>Persian.Plus</th><th>DNTPersianUtils</th></tr>
</thead>
<tbody>
<tr><td>کد ملی</td><td>26.31</td><td>148.86</td><td>116.92</td></tr>
<tr><td>شناسه شرکت</td><td>26.27</td><td>294.98</td><td>140.25</td></tr>
<tr><td>موبایل</td><td>25.91</td><td>88.50</td><td>117.56</td></tr>
<tr><td>کد پستی</td><td>13.54</td><td>66.67</td><td>113.49</td></tr>
<tr><td>کارت بانکی</td><td>48.20</td><td>423.90</td><td>342.70</td></tr>
<tr><td>شبا</td><td>148.78</td><td>298.52</td><td>210.12</td></tr>
</tbody>
</table>
<!-- /bench-table:summary-time -->

### تخصیص حافظه (بایت)

<!-- bench-table:summary-alloc -->
<table dir="rtl">
<thead>
<tr><th>اعتبارسنج</th><th>IranValidator</th><th>Persian.Plus</th><th>DNTPersianUtils</th></tr>
</thead>
<tbody>
<tr><td>کد ملی</td><td>0</td><td>0</td><td>148</td></tr>
<tr><td>شناسه شرکت</td><td>0</td><td>5046</td><td>0</td></tr>
<tr><td>موبایل</td><td>0</td><td>0</td><td>0</td></tr>
<tr><td>کد پستی</td><td>0</td><td>0</td><td>0</td></tr>
<tr><td>کارت بانکی</td><td>0</td><td>232</td><td>0</td></tr>
<tr><td>شبا</td><td>0</td><td>0</td><td>160</td></tr>
</tbody>
</table>
<!-- /bench-table:summary-alloc -->

**نکته‌های مهم:**

- تخصیص صفر مربوط به مسیر سریع است: ورودی‌هایی که از قبل نرمال هستند (`ASCII` خالص — حالت رایج) بدون کپی پردازش می‌شوند؛ ورودی‌های فارسی/عربی نیز به‌درستی نرمال‌سازی و اعتبارسنجی می‌شوند.
- برخلاف رقبا، IranValidator به‌جای `bool`، نتیجهٔ ساختاریافتهٔ `ValidationResult` برمی‌گرداند (`readonly struct` — بدون هزینهٔ `heap`) و اعداد فارسی/عربی را نرمال‌سازی می‌کند؛ Persian.Plus نرمال‌سازی نمی‌کند و ورودی غیر-ASCII را رد می‌کند.

جزئیات کامل: [راهنمای بنچمارک](benchmarks/README.md)

## پکیج‌های پروژه

<table dir="rtl">
<thead>
<tr><th>پکیج</th><th>وضعیت</th><th>کاربرد</th></tr>
</thead>
<tbody>
<tr><td>IranValidator.Core</td><td>✅</td><td>هسته اصلی اعتبارسنجی — بدون وابستگی</td></tr>
<tr><td>IranValidator.DataAnnotations</td><td>✅</td><td>ویژگی‌های اعتبارسنجی بر پایهٔ ویژگی‌ها</td></tr>
<tr><td>IranValidator.FluentValidation</td><td>✅</td><td>متدهای توسعه برای اعتبارسنجی روان</td></tr>
<tr><td>IranValidator.AspNetCore</td><td>✅</td><td>ثبت وابستگی، فیلتر اکشن و میدلور</td></tr>
<tr><td>IranValidator.MinimalApis</td><td>✅</td><td>سرویس تزریق‌پذیر برای برنامه‌های مینیمال</td></tr>
<tr><td>IranValidator.Localization</td><td>✅</td><td>پیام‌های خطای دوزبانه</td></tr>
</tbody>
</table>

## تست و پوشش

- **تعداد تست‌ها: ۱۰۹۹** — همه سبز (`xunit v3` + `FluentAssertions`)
- پوشش کد: **۹۹.۷۱٪ خط / ۹۷.۸۵٪ شاخه** (آستانهٔ CI: ۹۵٪)

<table dir="rtl">
<thead>
<tr><th>ماژول</th><th>Line</th><th>Branch</th><th>Method</th></tr>
</thead>
<tbody>
<tr><td>Core</td><td>99.86%</td><td>99.26%</td><td>99.24%</td></tr>
<tr><td>DataAnnotations</td><td>100%</td><td>100%</td><td>100%</td></tr>
<tr><td>FluentValidation</td><td>100%</td><td>96.96%</td><td>100%</td></tr>
<tr><td>AspNetCore</td><td>100%</td><td>78.57%</td><td>100%</td></tr>
<tr><td>MinimalApis</td><td>100%</td><td>100%</td><td>100%</td></tr>
<tr><td>Localization</td><td>97.40%</td><td>92.85%</td><td>85.71%</td></tr>
</tbody>
</table>

## CI/CD

- **فایل `ci.yml`** — بیلد + تست + گیت پوشش (آستانهٔ ۹۵٪) در هر push/PR
- **فایل `benchmarks.yml`** — اجرای ماهانهٔ بنچمارک (میانهٔ ۳ اجرا) + مقایسه با baseline (شکست در صورت افت واقعی عملکرد) + اجرا پیش از Release
- **فایل `release.yml`** — انتشار خودکار به NuGet.org و ساخت GitHub Release روی تگ‌های `v*`

## لایسنس

این پروژه تحت [لایسنس MIT](LICENSE) منتشر شده است.

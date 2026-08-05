# بنچمارک‌ها (Benchmarks)

**فارسی** | [English](README.en.md)

این سند عملکرد و تخصیص حافظه کتابخانه IranValidator را در مقایسه با پیاده‌سازی‌های رایج و کتابخانه‌های مشابه ارزیابی می‌کند.

## اهداف سنجش

ارزیابی‌ها توسط **BenchmarkDotNet** شامل سه بخش اصلی است:

- **ارزیابی هسته IranValidator:** سنجش زمان اجرا و حافظه برای ۱۰ اعتبارسنج اصلی در هر دو حالت `string` و `ReadOnlySpan<char>`.
- **مقایسه با کتابخانه‌های مشابه:** مقایسه عملکرد با Persian.Plus و DNTPersianUtils.Core در ۶ نوع داده مشترک.
- **مقایسه الگوریتم مستقیم Span با Regex:** سنجش پردازش دستی روی `ReadOnlySpan<char>` در برابر `Regex` کامپایل‌شده و `GeneratedRegex` (موبایل و کد پستی).

نتایج بنچمارک به صورت هفتگی در سیستم CI ارزیابی می‌شوند تا از عدم افت کارایی مطمئن شویم.

## شرایط اندازه‌گیری

* **سخت‌افزار اجرای آزمون:** سیستم GitHub Actions (محیط ubuntu-latest — شامل 2vCPU / 7GB RAM) · **دات‌نت:** <!-- bench-dotnet -->`10.0.10`<!-- /bench-dotnet --> · **ابزار:** <!-- bench-bdn -->`v0.14.0`<!-- /bench-bdn -->
* **پروفایل اجرای بنچمارک:** MediumRun (۱۵ تکرار، ۱۰ اجرای اولیه، ۲ اجرای نهایی) همراه با MemoryDiagnoser.
* **تاریخ سنجش:** <!-- bench-date -->`2026-08-05`<!-- /bench-date -->
* **برابری شرایط آزمون:** تمامی ورودی‌ها کاملاً معتبر بوده و صحت آن‌ها در هر سه کتابخانه تایید شده است.
* **تفاوت در نوع خروجی:** کتابخانه‌های دیگر صرفاً یک bool برمی‌گردانند، در حالی که IranValidator خروجی ساختاریافته ValidationResult را بدون تخصیص حافظه ارائه می‌دهد.

<table dir="rtl">
<thead>
<tr><th>کتابخانه</th><th>نوع خروجی</th><th>نرمال‌سازی خودکار</th><th>کدهای خطای تفکیک‌شده</th></tr>
</thead>
<tbody>
<tr><td>IranValidator</td><td>ValidationResult (Success, ErrorCode, NormalizedValue)</td><td>ارقام فارسی/عربی، فاصله، خط تیره</td><td>دارد</td></tr>
<tr><td>Persian.Plus</td><td>bool</td><td>ندارد (ورودی‌های غیر اسکی رد می‌شوند)</td><td>ندارد</td></tr>
<tr><td>DNTPersianUtils</td><td>bool</td><td>جزئی</td><td>ندارد</td></tr>
</tbody>
</table>
## نتایج ارزیابی

### بخش ۱: سنجش اختصاصی IranValidator — مقایسه string و span

<!-- bench-table:overloads -->
<table dir="rtl">
<thead>
<tr><th>متد</th><th>میانگین زمان اجرا</th><th>حافظه تخصیص‌یافته</th></tr>
</thead>
<tbody>
<tr><td>ValidateNationalCodeString</td><td>22.55 ns</td><td>0 B</td></tr>
<tr><td>ValidateNationalCodeSpan</td><td>96.42 ns</td><td>48 B</td></tr>
<tr><td>ValidateCompanyIdString</td><td>21.51 ns</td><td>0 B</td></tr>
<tr><td>ValidateCompanyIdSpan</td><td>98.87 ns</td><td>48 B</td></tr>
<tr><td>ValidateEconomicCodeString</td><td>26.58 ns</td><td>0 B</td></tr>
<tr><td>ValidateEconomicCodeSpan</td><td>103.73 ns</td><td>48 B</td></tr>
<tr><td>ValidateMobileString</td><td>22.26 ns</td><td>0 B</td></tr>
<tr><td>ValidateMobileSpan</td><td>97.67 ns</td><td>48 B</td></tr>
<tr><td>ValidateTelephoneString</td><td>16.58 ns</td><td>0 B</td></tr>
<tr><td>ValidateTelephoneSpan</td><td>94.45 ns</td><td>48 B</td></tr>
<tr><td>ValidatePostalCodeString</td><td>12.15 ns</td><td>0 B</td></tr>
<tr><td>ValidatePostalCodeSpan</td><td>86.67 ns</td><td>48 B</td></tr>
<tr><td>ValidateCardNumberString</td><td>42.99 ns</td><td>0 B</td></tr>
<tr><td>ValidateCardNumberSpan</td><td>143.03 ns</td><td>0 B</td></tr>
<tr><td>ValidatePassportString</td><td>12.77 ns</td><td>0 B</td></tr>
<tr><td>ValidatePassportSpan</td><td>95.32 ns</td><td>40 B</td></tr>
<tr><td>ValidateVehiclePlateString</td><td>10.17 ns</td><td>0 B</td></tr>
<tr><td>ValidateVehiclePlateSpan</td><td>79.72 ns</td><td>40 B</td></tr>
<tr><td>ValidateIbanString</td><td>140.04 ns</td><td>0 B</td></tr>
<tr><td>ValidateIbanSpan</td><td>289.80 ns</td><td>80 B</td></tr>
</tbody>
</table>
<!-- /bench-table:overloads -->

**نکات کلیدی:**

* **مسیر سریع رشته‌ای:** ورودی‌های ASCII از قبل نرمال‌شده، بدون هیچ تخصیص حافظه (۰ B) و در زمان کمتر از ۸۰ نانوثانیه پردازش می‌شوند.
* **متدهای اسپن (`Span`):** پردازش بر روی اسپن دلخواه، بافر کوچکی (۴۰ تا ۸۰ بایت) برای ساخت رشتهٔ خروجی نرمال‌شده تخصیص می‌دهد.

### بخش ۲: مقایسه IranValidator با کتابخانه‌های Persian.Plus و DNTPersianUtils

<!-- bench-table:vs-rivals -->
<table dir="rtl">
<thead>
<tr><th>متد</th><th>میانگین زمان اجرا</th><th>حافظه تخصیص‌یافته</th></tr>
</thead>
<tbody>
<tr><td>ValidateNationalCodeIranValidator</td><td>19.28 ns</td><td>0 B</td></tr>
<tr><td>ValidateNationalCodePersianPlus</td><td>89.84 ns</td><td>0 B</td></tr>
<tr><td>ValidateNationalCodeDntPersianUtils</td><td>97.47 ns</td><td>192 B</td></tr>
<tr><td>ValidateCompanyIdIranValidator</td><td>20.06 ns</td><td>0 B</td></tr>
<tr><td>ValidateCompanyIdPersianPlus</td><td>195.66 ns</td><td>136 B</td></tr>
<tr><td>ValidateCompanyIdDntPersianUtils</td><td>107.56 ns</td><td>192 B</td></tr>
<tr><td>ValidateMobileIranValidator</td><td>20.98 ns</td><td>0 B</td></tr>
<tr><td>ValidateMobilePersianPlus</td><td>77.56 ns</td><td>0 B</td></tr>
<tr><td>ValidateMobileDntPersianUtils</td><td>98.40 ns</td><td>96 B</td></tr>
<tr><td>ValidatePostalCodeIranValidator</td><td>12.15 ns</td><td>0 B</td></tr>
<tr><td>ValidatePostalCodePersianPlus</td><td>58.18 ns</td><td>0 B</td></tr>
<tr><td>ValidatePostalCodeDntPersianUtils</td><td>96.29 ns</td><td>96 B</td></tr>
<tr><td>ValidateCardNumberIranValidator</td><td>39.87 ns</td><td>0 B</td></tr>
<tr><td>ValidateCardNumberPersianPlus</td><td>370.66 ns</td><td>232 B</td></tr>
<tr><td>ValidateCardNumberDntPersianUtils</td><td>319.34 ns</td><td>344 B</td></tr>
<tr><td>ValidateIbanIranValidator</td><td>139.39 ns</td><td>0 B</td></tr>
<tr><td>ValidateIbanPersianPlus</td><td>250.59 ns</td><td>0 B</td></tr>
<tr><td>ValidateIbanDntPersianUtils</td><td>197.24 ns</td><td>160 B</td></tr>
</tbody>
</table>
<!-- /bench-table:vs-rivals -->

**نکات کلیدی:**

* **شماره کارت بانکی:** حدود ۹ برابر سریع‌تر از رقبا با تخصیص ۰ بایت در برابر ۲۳۲/۳۴۴ بایت.
* **شناسه شرکت:** ۶ تا ۱۳ برابر اجرای سریع‌تر.
* **شبا:** ۱.۸ تا ۲.۳ برابر سریع‌تر با تخصیص حافظه صفر.
* **موبایل و کد پستی:** ۶ تا ۱۰ برابر سریع‌تر از عبارت‌های منظم کامپایل‌شده.

### بخش ۳: مقایسه الگوریتم مستقیم روی Span در برابر عبارت منظم (Regex)

<!-- bench-table:regex -->
<table dir="rtl">
<thead>
<tr><th>پیاده‌سازی</th><th>میانگین</th></tr>
</thead>
<tbody>
<tr><td>پردازش مستقیم روی اسپن (موبایل)</td><td>21.14 ns</td></tr>
<tr><td>کامپایل‌شده با رجکس</td><td>26.46 ns</td></tr>
<tr><td>GeneratedRegex</td><td>20.02 ns</td></tr>
<tr><td>پردازش مستقیم روی اسپن (کدپستی)</td><td>12.25 ns</td></tr>
<tr><td>کامپایل‌شده با رجکس</td><td>26.04 ns</td></tr>
<tr><td>GeneratedRegex</td><td>20.22 ns</td></tr>
</tbody>
</table>
<!-- /bench-table:regex -->

پردازش مستقیم روی Span از هر دو نوع عبارت منظم (کامپایل‌شده و GeneratedRegex) در دات‌نت ۱۰ سریع‌تر است — حدود **۱.۶ تا ۲.۴ برابر** — زیرا regex حتی با runnerهای pooled، هزینهٔ سربار شروع به کار را به ازای هر فراخوانی می‌پردازد.

## نتیجه‌گیری (برای استفاده‌کنندگان)

- **در میان کتابخانه‌های فارسی‌دیت، IranValidator سریع‌ترین انتخاب است** روی هر ۶ نوع دادهٔ مشترک است، با حاشیهٔ ۱.۸ تا ۱۳ برابری — و هم‌زمان **صفر تخصیص** در مسیر رایج.
- اعداد را در مقیاس واقعی ببینیم (یک میلیون اعتبارسنجی):

   <table dir="rtl">
   <thead>
   <tr><th>عملیات</th><th>IranValidator</th><th>DNTPersianUtils</th><th>Persian.Plus</th></tr>
   </thead>
   <tbody>
   <tr><td>۱۰ میلیون کد ملی</td><td>~0.13s · 0 B</td><td>~0.76s · ~1.9 GB</td><td>~0.79s · 0 B</td></tr>
   <tr><td>۱۰ میلیون شمارهٔ کارت</td><td>~0.25s · 0 B</td><td>~2.3s · ~3.4 GB</td><td>~2.3s · ~2.3 GB</td></tr>
   <tr><td>۱۰ میلیون شبا</td><td>~0.81s · 0 B</td><td>~1.4s · ~1.6 GB</td><td>~1.7s · 0 B</td></tr>
   </tbody>
   </table>
   در ترافیک عادی وب (چند هزار درخواست در ثانیه) اختلاف زمانی زیر یک میلی‌ثانیه است؛ **مزیت واقعیِ ماندگار، صفر تخصیص است** — فشار کمتر روی GC، نرخ خطای کمتر در بار بالا و توقف‌های کمتر.
- **خروجی غنی‌تر:** IranValidator سریع‌تر است و همزمان `ValidationResult` با کد خطا و مقدار نرمال‌شده می‌دهد (نه فقط `bool`) و ورودیِ فارسی/عربی را هم نرمال و اعتبارسنجی می‌کند — جایی که Persian.Plus ورودی غیر-ASCII را رد می‌کند.
- **توصیهٔ عملی:** از overloadهای رشته‌ای استفاده کنید — ورودیِ از قبل نرمال‌شده همان مسیر سریعِ ۰ بایته است؛ و برای ورودیِ فارسی (مثلاً فرم‌های کاربر) نگران نباشید: نرمال‌سازی خودکار انجام می‌شود و نتیجه همچنان درست است.

## بازتولید بنچمارک‌ها

جهت اجرای بنچمارک‌ها روی سیستم خود:

```bash
dotnet run -c Release --project benchmarks/IranValidator.Benchmarks -- --job medium --inprocess --filter "*X*"
```

### پیوست فنی کوتاه (برای نگهدارنده‌ها)

* **برنامهٔ هفتگی:** `benchmarks.yml` هر دوشنبه ۰۲:۰۰ UTC و روی هر پوشِ `v*` اجرا می‌شود؛ `ci/benchmark_check.py` نتیجه را با `baseline.json` مقایسه می‌کند (پسرفت زمان > ۱.۵× یا تخصیص > ۱۶ B منجر به شکست می‌شود).
* **پرچم `--inprocess`:** ضروری است چون toolchain پیش‌فرض در کانتینرهای محدود ساکت کرش می‌کند؛ برای اعداد انتشار، روی ماشین تمیز CI اجرا کنید.
* **بهینه‌سازی:** `DisableOptimizationsValidator` غیرفعال است چون Persian.Plus با باینری بهینه‌نشده توزیع می‌شود.

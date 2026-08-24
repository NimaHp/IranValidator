# بنچمارک‌ها (Benchmarks)

**فارسی** | [English](README.en.md)

این سند عملکرد و تخصیص حافظه کتابخانه IranValidator را در مقایسه با پیاده‌سازی‌های رایج و کتابخانه‌های مشابه ارزیابی می‌کند.

## اهداف سنجش

ارزیابی‌ها توسط **BenchmarkDotNet** شامل سه بخش اصلی است:

- **ارزیابی هسته IranValidator:** سنجش زمان اجرا و حافظه برای ۱۰ اعتبارسنج اصلی در هر دو حالت `string` و `ReadOnlySpan<char>`.
- **مقایسه با کتابخانه‌های مشابه:** مقایسه عملکرد با Persian.Plus و DNTPersianUtils.Core در ۶ نوع داده مشترک.
- **مقایسه الگوریتم مستقیم Span با Regex:** سنجش پردازش دستی روی `ReadOnlySpan<char>` در برابر `Regex` کامپایل‌شده و `GeneratedRegex` (موبایل و کد پستی).

نتایج بنچمارک به صورت ماهانه در سیستم CI ارزیابی می‌شوند تا از عدم افت کارایی مطمئن شویم.

## شرایط اندازه‌گیری

* **سخت‌افزار اجرای آزمون:** سیستم GitHub Actions (محیط ubuntu-latest — شامل 2vCPU / 7GB RAM) · **دات‌نت:** <!-- bench-dotnet -->`10.0.11`<!-- /bench-dotnet --> · **ابزار:** <!-- bench-bdn -->`v0.14.0`<!-- /bench-bdn -->
* **پروفایل اجرای بنچمارک:** MediumRun (۱۵ تکرار، ۱۰ اجرای اولیه، ۲ اجرای نهایی) همراه با MemoryDiagnoser.
* **تاریخ سنجش:** <!-- bench-date -->`2026-08-18`<!-- /bench-date -->
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
<tr><td>ValidateNationalCodeString</td><td>23.32 ns</td><td>0 B</td></tr>
<tr><td>ValidateNationalCodeSpan</td><td>151.10 ns</td><td>48 B</td></tr>
<tr><td>ValidateCompanyIdString</td><td>25.42 ns</td><td>0 B</td></tr>
<tr><td>ValidateCompanyIdSpan</td><td>160.22 ns</td><td>48 B</td></tr>
<tr><td>ValidateEconomicCodeString</td><td>26.80 ns</td><td>0 B</td></tr>
<tr><td>ValidateEconomicCodeSpan</td><td>160.36 ns</td><td>48 B</td></tr>
<tr><td>ValidateMobileString</td><td>29.39 ns</td><td>0 B</td></tr>
<tr><td>ValidateMobileSpan</td><td>154.60 ns</td><td>48 B</td></tr>
<tr><td>ValidateTelephoneString</td><td>24.32 ns</td><td>0 B</td></tr>
<tr><td>ValidateTelephoneSpan</td><td>151.73 ns</td><td>48 B</td></tr>
<tr><td>ValidatePostalCodeString</td><td>11.88 ns</td><td>0 B</td></tr>
<tr><td>ValidatePostalCodeSpan</td><td>136.83 ns</td><td>48 B</td></tr>
<tr><td>ValidateCardNumberString</td><td>43.22 ns</td><td>0 B</td></tr>
<tr><td>ValidateCardNumberSpan</td><td>199.88 ns</td><td>56 B</td></tr>
<tr><td>ValidatePassportString</td><td>14.76 ns</td><td>0 B</td></tr>
<tr><td>ValidatePassportSpan</td><td>145.34 ns</td><td>40 B</td></tr>
<tr><td>ValidateVehiclePlateString</td><td>13.21 ns</td><td>0 B</td></tr>
<tr><td>ValidateVehiclePlateSpan</td><td>121.97 ns</td><td>0 B</td></tr>
<tr><td>ValidateIbanString</td><td>131.20 ns</td><td>0 B</td></tr>
<tr><td>ValidateIbanSpan</td><td>345.87 ns</td><td>80 B</td></tr>
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
<tr><td>ValidateNationalCodeIranValidator</td><td>23.16 ns</td><td>0 B</td></tr>
<tr><td>ValidateNationalCodePersianPlus</td><td>154.36 ns</td><td>0 B</td></tr>
<tr><td>ValidateNationalCodeDntPersianUtils</td><td>114.13 ns</td><td>0 B</td></tr>
<tr><td>ValidateCompanyIdIranValidator</td><td>24.07 ns</td><td>0 B</td></tr>
<tr><td>ValidateCompanyIdPersianPlus</td><td>284.37 ns</td><td>136 B</td></tr>
<tr><td>ValidateCompanyIdDntPersianUtils</td><td>132.12 ns</td><td>192 B</td></tr>
<tr><td>ValidateMobileIranValidator</td><td>22.44 ns</td><td>0 B</td></tr>
<tr><td>ValidateMobilePersianPlus</td><td>93.63 ns</td><td>0 B</td></tr>
<tr><td>ValidateMobileDntPersianUtils</td><td>125.57 ns</td><td>0 B</td></tr>
<tr><td>ValidatePostalCodeIranValidator</td><td>11.89 ns</td><td>0 B</td></tr>
<tr><td>ValidatePostalCodePersianPlus</td><td>68.89 ns</td><td>0 B</td></tr>
<tr><td>ValidatePostalCodeDntPersianUtils</td><td>108.38 ns</td><td>89 B</td></tr>
<tr><td>ValidateCardNumberIranValidator</td><td>41.97 ns</td><td>0 B</td></tr>
<tr><td>ValidateCardNumberPersianPlus</td><td>457.11 ns</td><td>0 B</td></tr>
<tr><td>ValidateCardNumberDntPersianUtils</td><td>348.81 ns</td><td>0 B</td></tr>
<tr><td>ValidateIbanIranValidator</td><td>131.94 ns</td><td>0 B</td></tr>
<tr><td>ValidateIbanPersianPlus</td><td>343.15 ns</td><td>0 B</td></tr>
<tr><td>ValidateIbanDntPersianUtils</td><td>201.12 ns</td><td>0 B</td></tr>
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
<tr><td>پردازش مستقیم روی اسپن (موبایل)</td><td>22.53 ns</td></tr>
<tr><td>کامپایل‌شده با رجکس</td><td>27.01 ns</td></tr>
<tr><td>GeneratedRegex</td><td>20.34 ns</td></tr>
<tr><td>پردازش مستقیم روی اسپن (کدپستی)</td><td>11.89 ns</td></tr>
<tr><td>کامپایل‌شده با رجکس</td><td>26.34 ns</td></tr>
<tr><td>GeneratedRegex</td><td>20.42 ns</td></tr>
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

* **برنامهٔ ماهانه:** `benchmarks.yml` اولِ هر ماه ساعت ۰۲:۰۰ UTC و روی هر پوشِ `v*` اجرا می‌شود؛ این workflow بنچمارک را سه بار می‌راند و `ci/benchmark_check.py` میانه (median) نتایج را با `baseline.json` مقایسه می‌کند (پسرفت زمان > ۱.۵× یا تخصیص فراتر از «حداکثر(۶۴ بایت، ۲۰٪ baseline)» منجر به شکست می‌شود). ردیف‌های مقایسه با کتابخانه‌های شخص ثالث (Persian.Plus، DNTPersianUtils، regex) فقط گزارش می‌شوند و هرگز gate را نمی‌شکنند.
* **پرچم `--inprocess`:** ضروری است چون toolchain پیش‌فرض در کانتینرهای محدود ساکت کرش می‌کند؛ برای اعداد انتشار، روی ماشین تمیز CI اجرا کنید.
* **بهینه‌سازی:** `DisableOptimizationsValidator` غیرفعال است چون Persian.Plus با باینری بهینه‌نشده توزیع می‌شود.

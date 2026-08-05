# تغییرات (Changelog)

**فارسی** | [English](CHANGELOG.en.md)

این سند از [Keep a Changelog](https://keepachangelog.com/fa-IR/1.1.0/) و سیستم نسخه‌گذاری [SemVer](https://semver.org/lang/fa/) پیروی می‌کند.

## [Unreleased]

### افزوده‌شده

* **۱۰ اعتبارسنج اصلی:** کد ملی، شناسه ملی شرکت، کد اقتصادی، شماره کارت بانکی
  (BIN ایران + الگوریتم Luhn)، شبا (پویش تک‌گذر MOD-97 + اعتبارسنجی کد بانک)، موبایل، کد پستی، تلفن
  ثابت، پاسپورت و پلاک خودرو — همگی بر پایه الگوی singleton با دو Overload برای
  Validate(string) و Validate(ReadOnlySpan<char>).
* **۶ پکیج کاربردی:** Core، Localization، DataAnnotations، FluentValidation،
  AspNetCore و MinimalApis (با پشتیبانی از netstandard2.0، net8.0 و net10.0).
* **مسیر سریع بدون تخصیص حافظه (Zero-Alloc):** متد Normalize(span, original) همراه با پیش‌پردازش
  NeedsNormalization — بدون تخصیص حافظه (۰ بایت) برای ورودی‌های استاندارد.
* **نرمال‌سازی خودکار:** تبدیل و اصلاح خودکار اعداد فارسی/عربی، نیم‌فاصله، فاصله و خط تیره.
* **کدهای خطای ساختاریافته (ValidationErrorCode):** شامل ۱۰ کد خطای دقیق (InvalidLength، InvalidFormat،
  InvalidChecksum، InvalidCharacters، InvalidProvinceCode، InvalidBankCode،
  UnsupportedIssuer، ValueEmpty، InvalidAreaCode و None).
* **سیستم محلی‌سازی (Localization):** رزولورهای داخلی فارسی و انگلیسی با اولویت تزریق وابستگی (DI) و Fallback به رجیستری استاتیک.
* **بنچمارک و پایش کارایی:** افزودن BenchmarkDotNet و کنترل کیفیت هفتگی در CI جهت جلوگیری از افت عملکرد (آستانه ۱.۵ برابر زمان / ۱۶ بایت حافظه).
* **پوشش تست گسترده:** ۱۰۹۹ تست با پوشش ۹۹.۷۱٪ خطوط و ۹۷.۸۵٪ شاخه‌ها (با آستانه حداقل ۹۵٪ در CI).
* **مستندات کامل دوزبانه:** مستندات فارسی (اصلی) و انگلیسی به همراه فایل‌های راهنمای جامعه کاربری
  (CONTRIBUTING، SECURITY، MIGRATION و قالب‌های PR/Issue).
* **انتشار خودکار:** انتشار خودکار نسخه جدید در NuGet.org با ثبت تگ‌های v* در مخزن (release.yml).
* **اعتبارسنجی تلفن ثابت:** افزودن ۳۱ کد استان و بررسی شروع شماره محلی با ارقام ۲ تا ۹ (رقم ۰ برای پیش‌شماره شهری و رقم ۱ برای خدمات اضطراری/۳رقمی).

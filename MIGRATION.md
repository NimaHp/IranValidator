# راهنمای مهاجرت (Migration)

**فارسی** | [English](MIGRATION.en.md)

## مهاجرت از Persian.Plus یا DNTPersianUtils.Core

جدول زیر جایگزین‌های سیستم IranValidator را برای کتابخانه‌های متداول نمایش می‌دهد:

| کارکرد | Persian.Plus | DNTPersianUtils.Core | IranValidator |
| :--- | :--- | :--- | :--- |
| کد ملی | IranianNationalCode.Validate(v) | IranianNationalId.Validate(v) | NationalCodeValidator.Instance.Validate(v) |
| شماره کارت | IranianCardNumber.Validate(v) | IranianCardNumber.Validate(v) | CardNumberValidator.Instance.Validate(v) |
| موبایل | IranianMobile.Validate(v) | IranianMobile.Validate(v) | MobileValidator.Instance.Validate(v) |
| کد پستی | IranianPostalCode.Validate(v) | IranianPostalCode.Validate(v) | PostalCodeValidator.Instance.Validate(v) |
| شبا | — | IranianSheba.Validate(v) | IbanValidator.Instance.Validate(v) |
| تلفن ثابت | — | — | TelephoneValidator.Instance.Validate(v) |
| پاسپورت | — | — | PassportValidator.Instance.Validate(v) |
| پلاک خودرو | — | — | VehiclePlateValidator.Instance.Validate(v) |
| شناسه ملی شرکت | — | — | CompanyIdValidator.Instance.Validate(v) |
| کد اقتصادی | — | — | EconomicCodeValidator.Instance.Validate(v) |

> نام کلاس‌های ایستا در نسخه‌های مختلف کتابخانه‌های قدیمی ممکن است متفاوت باشد؛ ستون IranValidator مرجع اصلی پروژه است.

## تفاوت‌های رفتاری

1. **نوع خروجی:** به جای bool ساده، خروجی به صورت یک ساختار سبک ValidationResult (مبتنی بر readonly struct) برگردانده می‌شود که شامل result.Success، result.ErrorCode و result.NormalizedValue است.
2. **نرمال‌سازی خودکار:** ارقام فارسی/عربی، نیم‌فاصله، فاصله و خط تیره پیش از اعتبارسنجی اصلاح می‌شوند — بنابراین نیازی به مبدل‌های دستی مانند (ToEnglishNumber) نیست و ورودی‌هایی مانند "۰۹۱۲-۱۲۳ ۴۵۶۷" معتبر شناخته می‌شوند.
3. **مدیریت ورودی‌های Null و خالی:** مقادیر null یا خالی طبق استاندارد DataAnnotations معتبر در نظر گرفته می‌شوند. در صورت اجباری بودن ورودی، بررسی آن را به صورت جداگانه انجام دهید.
4. **کدهای خطای تفکیک‌شده:** ارائه ۱۰ کد خطای دقیق در enum ValidationErrorCode برای مدیریت بهتر پیام‌ها (مثلاً کد بانک نادرست در شماره شبا → InvalidBankCode).
5. **ایمنی در محیط چندنخی (Thread-Safe):** تمامی اعتبارسنج‌ها به صورت Singleton بدون حالت (Stateless) پیاده‌سازی شده‌اند.

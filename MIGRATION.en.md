# Migration Guide

**English** | [فارسی](MIGRATION.md)

## Migrating from Persian.Plus or DNTPersianUtils.Core

This table maps common operations to their IranValidator equivalents:

| Concern | Persian.Plus | DNTPersianUtils.Core | IranValidator |
| :--- | :--- | :--- | :--- |
| National Code | IranianNationalCode.Validate(v) | IranianNationalId.Validate(v) | NationalCodeValidator.Instance.Validate(v) |
| Card Number | IranianCardNumber.Validate(v) | IranianCardNumber.Validate(v) | CardNumberValidator.Instance.Validate(v) |
| Mobile | IranianMobile.Validate(v) | IranianMobile.Validate(v) | MobileValidator.Instance.Validate(v) |
| Postal Code | IranianPostalCode.Validate(v) | IranianPostalCode.Validate(v) | PostalCodeValidator.Instance.Validate(v) |
| IBAN | — | IranianSheba.Validate(v) | IbanValidator.Instance.Validate(v) |
| Landline | — | — | TelephoneValidator.Instance.Validate(v) |
| Passport | — | — | PassportValidator.Instance.Validate(v) |
| Vehicle Plate | — | — | VehiclePlateValidator.Instance.Validate(v) |
| Company ID | — | — | CompanyIdValidator.Instance.Validate(v) |
| Economic Code | — | — | EconomicCodeValidator.Instance.Validate(v) |

> Exact static class names may vary across versions of legacy libraries; the IranValidator column is the primary reference.

## Key Behavioral Differences

1. **Return Type:** Instead of returning a primitive bool, IranValidator returns a lightweight, read-only ValidationResult struct containing result.Success, result.ErrorCode, and result.NormalizedValue.
2. **Automatic Normalization:** Persian/Arabic digits, zero-width spaces (ZWNJ), spaces, and dashes are automatically normalized prior to validation — eliminating the need for explicit manual conversions (ToEnglishNumber). For example, "۰۹۱۲-۱۲۳ ۴۵۶۷" is evaluated as valid.
3. **Null and Empty Handling:** null and empty strings are treated as valid by convention (matching DataAnnotations behavior). Enforce presence separately when a field is required.
4. **Structured Error Codes:** 10 discrete codes in ValidationErrorCode offer granular feedback (e.g., an IBAN with an invalid bank code produces InvalidBankCode).
5. **Stateless & Thread-Safe:** All validator singletons maintain no state, ensuring safe concurrent execution.

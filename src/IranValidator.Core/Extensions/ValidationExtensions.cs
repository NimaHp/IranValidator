using IranValidator.Core.Results;
using IranValidator.Core.Validators;

namespace IranValidator.Core.Extensions;

/// <summary>
/// Extension methods for string validation.
/// </summary>
public static class ValidationExtensions
{
    // === National Code ===

    /// <summary>
    /// Checks if the string is a valid Iranian national code.
    /// </summary>
    public static bool IsIranNationalCode(this string value)
        => NationalCodeValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian national code.
    /// </summary>
    public static ValidationResult ValidateIranNationalCode(this string value)
        => NationalCodeValidator.Instance.Validate(value);

    // === Mobile ===

    /// <summary>
    /// Checks if the string is a valid Iranian mobile number.
    /// </summary>
    public static bool IsIranMobile(this string value)
        => MobileValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian mobile number.
    /// </summary>
    public static ValidationResult ValidateIranMobile(this string value)
        => MobileValidator.Instance.Validate(value);

    // === Postal Code ===

    /// <summary>
    /// Checks if the string is a valid Iranian postal code.
    /// </summary>
    public static bool IsIranPostalCode(this string value)
        => PostalCodeValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian postal code.
    /// </summary>
    public static ValidationResult ValidateIranPostalCode(this string value)
        => PostalCodeValidator.Instance.Validate(value);

    // === Telephone ===

    /// <summary>
    /// Checks if the string is a valid Iranian landline telephone number.
    /// </summary>
    public static bool IsIranTelephone(this string value)
        => TelephoneValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian landline telephone number.
    /// </summary>
    public static ValidationResult ValidateIranTelephone(this string value)
        => TelephoneValidator.Instance.Validate(value);

    // === IBAN ===

    /// <summary>
    /// Checks if the string is a valid Iranian IBAN (شبا).
    /// </summary>
    public static bool IsIranIban(this string value)
        => IbanValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian IBAN (شبا).
    /// </summary>
    public static ValidationResult ValidateIranIban(this string value)
        => IbanValidator.Instance.Validate(value);

    // === Card Number ===

    /// <summary>
    /// Checks if the string is a valid Iranian bank card number.
    /// </summary>
    public static bool IsIranCardNumber(this string value)
        => CardNumberValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian bank card number.
    /// </summary>
    public static ValidationResult ValidateIranCardNumber(this string value)
        => CardNumberValidator.Instance.Validate(value);

    // === Company Id ===

    /// <summary>
    /// Checks if the string is a valid Iranian Company ID (شناسه ملی شرکت).
    /// </summary>
    public static bool IsIranCompanyId(this string value)
        => CompanyIdValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian Company ID (شناسه ملی شرکت).
    /// </summary>
    public static ValidationResult ValidateIranCompanyId(this string value)
        => CompanyIdValidator.Instance.Validate(value);

    // === Economic Code ===

    /// <summary>
    /// Checks if the string is a valid Iranian Economic Code (کد اقتصادی).
    /// </summary>
    public static bool IsIranEconomicCode(this string value)
        => EconomicCodeValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian Economic Code (کد اقتصادی).
    /// </summary>
    public static ValidationResult ValidateIranEconomicCode(this string value)
        => EconomicCodeValidator.Instance.Validate(value);

    // === Passport ===

    /// <summary>
    /// Checks if the string is a valid Iranian Passport Number (شماره گذرنامه).
    /// </summary>
    public static bool IsIranPassport(this string value)
        => PassportValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian Passport Number (شماره گذرنامه).
    /// </summary>
    public static ValidationResult ValidateIranPassport(this string value)
        => PassportValidator.Instance.Validate(value);

    // === Vehicle Plate ===

    /// <summary>
    /// Checks if the string is a valid Iranian Vehicle Plate (پلاک خودرو).
    /// </summary>
    public static bool IsIranVehiclePlate(this string value)
        => VehiclePlateValidator.Instance.Validate(value).Success;

    /// <summary>
    /// Validates the string as an Iranian Vehicle Plate (پلاک خودرو).
    /// </summary>
    public static ValidationResult ValidateIranVehiclePlate(this string value)
        => VehiclePlateValidator.Instance.Validate(value);
}

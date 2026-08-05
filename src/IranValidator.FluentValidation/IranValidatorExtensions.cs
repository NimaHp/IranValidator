using FluentValidation;
using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.FluentValidation;

/// <summary>
/// FluentValidation extension methods for Persian data validation.
/// </summary>
public static class IranValidatorExtensions
{
    /// <summary>
    /// Validates that the string is a valid Iranian national code (کد ملی).
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranNationalCode<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || NationalCodeValidator.Instance.Validate(value).Success),
            NationalCodeValidator.Instance);
    }

    /// <summary>
    /// Validates that the string is a valid Iranian mobile number.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranMobile<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || MobileValidator.Instance.Validate(value).Success),
            MobileValidator.Instance);
    }

    /// <summary>
    /// Validates that the string is a valid Iranian postal code.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranPostalCode<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || PostalCodeValidator.Instance.Validate(value).Success),
            PostalCodeValidator.Instance);
    }

    /// <summary>
    /// Validates that the string is a valid Iranian landline telephone number.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranTelephone<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || TelephoneValidator.Instance.Validate(value).Success),
            TelephoneValidator.Instance);
    }

    /// <summary>
    /// Validates that the string is a valid Iranian bank card number.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranCardNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || CardNumberValidator.Instance.Validate(value).Success),
            CardNumberValidator.Instance);
    }

    /// <summary>
    /// Validates that the string is a valid Iranian Company ID (شناسه ملی شرکت).
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranCompanyId<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || CompanyIdValidator.Instance.Validate(value).Success),
            CompanyIdValidator.Instance);
    }

    /// <summary>
    /// Validates that the string is a valid Iranian Economic Code (کد اقتصادی).
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranEconomicCode<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || EconomicCodeValidator.Instance.Validate(value).Success),
            EconomicCodeValidator.Instance);
    }

    /// <summary>
    /// Validates that the string is a valid Iranian Passport Number (شماره گذرنامه).
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranPassport<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || PassportValidator.Instance.Validate(value).Success),
            PassportValidator.Instance);
    }

    /// <summary>
    /// Validates that the string is a valid Iranian Vehicle Plate (پلاک خودرو).
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranVehiclePlate<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || VehiclePlateValidator.Instance.Validate(value).Success),
            VehiclePlateValidator.Instance);
    }

    /// <summary>
    /// Validates that the string is a valid Iranian IBAN (شماره شبا).
    /// </summary>
    public static IRuleBuilderOptions<T, string?> IranIban<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return WithLocalizedMessage(
            ruleBuilder.Must(value => value is null || value.Length == 0 || IbanValidator.Instance.Validate(value).Success),
            IbanValidator.Instance);
    }

    /// <summary>
    /// Replaces the rule's default message with a localized message resolved at
    /// validation time. The specific error code of the underlying validator is
    /// used (e.g. a checksum failure produces the "invalid checksum" message)
    /// and the property name comes from FluentValidation's message context.
    /// Note: the message is built only for failures, so the underlying
    /// validator runs twice for invalid values (once in the predicate, once
    /// for the message) — a negligible cost on the rare failure path.
    /// </summary>
    private static IRuleBuilderOptions<T, string?> WithLocalizedMessage<T>(
        IRuleBuilderOptions<T, string?> rule,
        IStringValidator validator)
    {
        return rule.Configure(ruleConfig =>
            ruleConfig.MessageBuilder = ctx =>
                IranFluentValidationLocalization.GetMessage(
                    validator.Validate((string)ctx.PropertyValue!).ErrorCode,
                    ctx.PropertyName));
    }
}

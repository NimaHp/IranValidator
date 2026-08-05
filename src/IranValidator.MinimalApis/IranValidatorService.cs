using IranValidator.Core.Results;
using IranValidator.Core.Validators;

namespace IranValidator.MinimalApis;

/// <summary>
/// Injectable service that wraps all IranValidator.Core validators for use in Minimal API endpoints.
/// </summary>
public sealed class IranValidatorService
{
    private readonly NationalCodeValidator _nationalCode = NationalCodeValidator.Instance;
    private readonly MobileValidator _mobile = MobileValidator.Instance;
    private readonly PostalCodeValidator _postalCode = PostalCodeValidator.Instance;
    private readonly TelephoneValidator _telephone = TelephoneValidator.Instance;
    private readonly CardNumberValidator _cardNumber = CardNumberValidator.Instance;
    private readonly CompanyIdValidator _companyId = CompanyIdValidator.Instance;
    private readonly EconomicCodeValidator _economicCode = EconomicCodeValidator.Instance;
    private readonly PassportValidator _passport = PassportValidator.Instance;
    private readonly VehiclePlateValidator _vehiclePlate = VehiclePlateValidator.Instance;
    private readonly IbanValidator _iban = IbanValidator.Instance;

    /// <summary>
    /// Validates an Iranian national code (کد ملی).
    /// </summary>
    public ValidationResult ValidateNationalCode(string value)
        => _nationalCode.Validate(value);

    /// <summary>
    /// Validates an Iranian mobile number.
    /// </summary>
    public ValidationResult ValidateMobile(string value)
        => _mobile.Validate(value);

    /// <summary>
    /// Validates an Iranian postal code.
    /// </summary>
    public ValidationResult ValidatePostalCode(string value)
        => _postalCode.Validate(value);

    /// <summary>
    /// Validates an Iranian landline telephone number.
    /// </summary>
    public ValidationResult ValidateTelephone(string value)
        => _telephone.Validate(value);

    /// <summary>
    /// Validates an Iranian bank card number.
    /// </summary>
    public ValidationResult ValidateCardNumber(string value)
        => _cardNumber.Validate(value);

    /// <summary>
    /// Validates an Iranian Company ID (شناسه ملی شرکت).
    /// </summary>
    public ValidationResult ValidateCompanyId(string value)
        => _companyId.Validate(value);

    /// <summary>
    /// Validates an Iranian Economic Code (کد اقتصادی).
    /// </summary>
    public ValidationResult ValidateEconomicCode(string value)
        => _economicCode.Validate(value);

    /// <summary>
    /// Validates an Iranian Passport Number (شماره گذرنامه).
    /// </summary>
    public ValidationResult ValidatePassport(string value)
        => _passport.Validate(value);

    /// <summary>
    /// Validates an Iranian Vehicle Plate (پلاک خودرو).
    /// </summary>
    public ValidationResult ValidateVehiclePlate(string value)
        => _vehiclePlate.Validate(value);

    /// <summary>
    /// Validates an Iranian IBAN (شماره شبا).
    /// </summary>
    public ValidationResult ValidateIban(string value)
        => _iban.Validate(value);
}

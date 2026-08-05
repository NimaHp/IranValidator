using BenchmarkDotNet.Attributes;
using IranValidator.Core.Validators;

namespace IranValidator.Benchmarks;

/// <summary>
/// Benchmarks all 10 IranValidator validators, comparing the string overload
/// against the ReadOnlySpan overload for each.
/// </summary>
[MemoryDiagnoser]
public class ValidatorBenchmarks
{
    // Valid inputs (taken from the test suite)
    private readonly string _nationalCode = "0010350829";
    private readonly string _companyId = "10380284752";
    private readonly string _economicCode = "123456789019";
    private readonly string _mobile = "09121234567";
    private readonly string _telephone = "02122345678";
    private readonly string _postalCode = "1145687654";
    private readonly string _cardNumber = "6037991234567893"; // Bank Melli Iran (Luhn-valid, Iranian BIN)
    private readonly string _passport = "P12345678";
    private readonly string _vehiclePlate = "12B34567";
    private readonly string _iban = "IR820540102680020817909002";

    // === National Code ===

    [Benchmark]
    public bool ValidateNationalCodeString()
        => NationalCodeValidator.Instance.Validate(_nationalCode).Success;

    [Benchmark]
    public bool ValidateNationalCodeSpan()
        => NationalCodeValidator.Instance.Validate(_nationalCode.AsSpan()).Success;

    // === Company ID ===

    [Benchmark]
    public bool ValidateCompanyIdString()
        => CompanyIdValidator.Instance.Validate(_companyId).Success;

    [Benchmark]
    public bool ValidateCompanyIdSpan()
        => CompanyIdValidator.Instance.Validate(_companyId.AsSpan()).Success;

    // === Economic Code ===

    [Benchmark]
    public bool ValidateEconomicCodeString()
        => EconomicCodeValidator.Instance.Validate(_economicCode).Success;

    [Benchmark]
    public bool ValidateEconomicCodeSpan()
        => EconomicCodeValidator.Instance.Validate(_economicCode.AsSpan()).Success;

    // === Mobile ===

    [Benchmark]
    public bool ValidateMobileString()
        => MobileValidator.Instance.Validate(_mobile).Success;

    [Benchmark]
    public bool ValidateMobileSpan()
        => MobileValidator.Instance.Validate(_mobile.AsSpan()).Success;

    // === Telephone ===

    [Benchmark]
    public bool ValidateTelephoneString()
        => TelephoneValidator.Instance.Validate(_telephone).Success;

    [Benchmark]
    public bool ValidateTelephoneSpan()
        => TelephoneValidator.Instance.Validate(_telephone.AsSpan()).Success;

    // === Postal Code ===

    [Benchmark]
    public bool ValidatePostalCodeString()
        => PostalCodeValidator.Instance.Validate(_postalCode).Success;

    [Benchmark]
    public bool ValidatePostalCodeSpan()
        => PostalCodeValidator.Instance.Validate(_postalCode.AsSpan()).Success;

    // === Card Number ===

    [Benchmark]
    public bool ValidateCardNumberString()
        => CardNumberValidator.Instance.Validate(_cardNumber).Success;

    [Benchmark]
    public bool ValidateCardNumberSpan()
        => CardNumberValidator.Instance.Validate(_cardNumber.AsSpan()).Success;

    // === Passport ===

    [Benchmark]
    public bool ValidatePassportString()
        => PassportValidator.Instance.Validate(_passport).Success;

    [Benchmark]
    public bool ValidatePassportSpan()
        => PassportValidator.Instance.Validate(_passport.AsSpan()).Success;

    // === Vehicle Plate ===

    [Benchmark]
    public bool ValidateVehiclePlateString()
        => VehiclePlateValidator.Instance.Validate(_vehiclePlate).Success;

    [Benchmark]
    public bool ValidateVehiclePlateSpan()
        => VehiclePlateValidator.Instance.Validate(_vehiclePlate.AsSpan()).Success;

    // === IBAN ===

    [Benchmark]
    public bool ValidateIbanString()
        => IbanValidator.Instance.Validate(_iban).Success;

    [Benchmark]
    public bool ValidateIbanSpan()
        => IbanValidator.Instance.Validate(_iban.AsSpan()).Success;
}

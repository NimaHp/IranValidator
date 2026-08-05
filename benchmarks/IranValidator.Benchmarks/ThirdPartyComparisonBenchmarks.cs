using BenchmarkDotNet.Attributes;
using IranValidator.Core.Validators;

// Aliases used to disambiguate the same-named extension/static methods
// shipped by Persian.Plus and DNTPersianUtils.Core.
using PP = Persian.Plus.Extensions;
using DNT = DNTPersianUtils.Core;

namespace IranValidator.Benchmarks;

/// <summary>
/// Compares IranValidator against Persian.Plus and DNTPersianUtils.Core
/// on the validator types all three libraries support.
/// </summary>
[MemoryDiagnoser]
public class ThirdPartyComparisonBenchmarks
{
    // Valid inputs that ALL THREE libraries accept (verified empirically).
    // The GlobalSetup guard below re-checks this on every run so the
    // comparison can never silently degrade into measuring rejection paths.
    private readonly string _nationalCode = "0010350829";
    private readonly string _companyId = "10380284752";
    private readonly string _mobile = "09121234567";
    private readonly string _postalCode = "1145687654";
    private readonly string _cardNumber = "6037991234567893"; // Bank Melli Iran (Luhn-valid, Iranian BIN)
    private readonly string _iban = "IR820540102680020817909002";

    [GlobalSetup]
    public void EnsureInputsValidForAllLibraries()
    {
        var failures = new List<string>();

        void Check(string name, string input, Func<string, bool> ours, Func<string, bool> persianPlus, Func<string, bool> dnt)
        {
            if (!ours(input))
                failures.Add($"{name} ({input}): invalid for IranValidator");
            if (!persianPlus(input))
                failures.Add($"{name} ({input}): invalid for Persian.Plus");
            if (!dnt(input))
                failures.Add($"{name} ({input}): invalid for DNTPersianUtils");
        }

        Check("NationalCode", _nationalCode,
            v => NationalCodeValidator.Instance.Validate(v).Success,
            PP.IranianNationalCodeExtensions.IsValidIranianNationalCode,
            DNT.NationalCodeUtils.IsValidIranianNationalCode);
        Check("CompanyId", _companyId,
            v => CompanyIdValidator.Instance.Validate(v).Success,
            PP.IranianNationalLegalCodeExtensions.IsValidIranianNationalLegalCode,
            DNT.NationalLegalCodeUtils.IsValidIranianNationalLegalCode);
        Check("Mobile", _mobile,
            v => MobileValidator.Instance.Validate(v).Success,
            PP.IranianMobileNumberExtensions.IsValidIranianMobileNumber,
            v => DNT.IranCodesUtils.IsValidIranianMobileNumber(v));
        Check("PostalCode", _postalCode,
            v => PostalCodeValidator.Instance.Validate(v).Success,
            PP.IranianPostalCodeExtensions.IsValidIranianPostalCode,
            v => DNT.IranCodesUtils.IsValidIranianPostalCode(v));
        Check("CardNumber", _cardNumber,
            v => CardNumberValidator.Instance.Validate(v).Success,
            PP.IranianShetabCardExtensions.IsValidIranianShetabCardNumber,
            v => DNT.IranShetabUtils.IsValidIranShetabNumber(v));
        Check("Iban", _iban,
            v => IbanValidator.Instance.Validate(v).Success,
            PP.IranianIbanExtensions.IsValidIranianIbanNumber,
            v => DNT.IranShebaUtils.IsValidIranShebaNumber(v));

        if (failures.Count > 0)
            throw new InvalidOperationException(
                "Benchmark inputs must be valid for all three libraries: " + string.Join("; ", failures));
    }

    // === National Code ===

    [Benchmark]
    public bool ValidateNationalCodeIranValidator()
        => NationalCodeValidator.Instance.Validate(_nationalCode).Success;

    [Benchmark]
    public bool ValidateNationalCodePersianPlus()
        => PP.IranianNationalCodeExtensions.IsValidIranianNationalCode(_nationalCode);

    [Benchmark]
    public bool ValidateNationalCodeDntPersianUtils()
        => DNT.NationalCodeUtils.IsValidIranianNationalCode(_nationalCode);

    // === Company ID (National Legal Code) ===

    [Benchmark]
    public bool ValidateCompanyIdIranValidator()
        => CompanyIdValidator.Instance.Validate(_companyId).Success;

    [Benchmark]
    public bool ValidateCompanyIdPersianPlus()
        => PP.IranianNationalLegalCodeExtensions.IsValidIranianNationalLegalCode(_companyId);

    [Benchmark]
    public bool ValidateCompanyIdDntPersianUtils()
        => DNT.NationalLegalCodeUtils.IsValidIranianNationalLegalCode(_companyId);

    // === Mobile ===

    [Benchmark]
    public bool ValidateMobileIranValidator()
        => MobileValidator.Instance.Validate(_mobile).Success;

    [Benchmark]
    public bool ValidateMobilePersianPlus()
        => PP.IranianMobileNumberExtensions.IsValidIranianMobileNumber(_mobile);

    [Benchmark]
    public bool ValidateMobileDntPersianUtils()
        => DNT.IranCodesUtils.IsValidIranianMobileNumber(_mobile);

    // === Postal Code ===

    [Benchmark]
    public bool ValidatePostalCodeIranValidator()
        => PostalCodeValidator.Instance.Validate(_postalCode).Success;

    [Benchmark]
    public bool ValidatePostalCodePersianPlus()
        => PP.IranianPostalCodeExtensions.IsValidIranianPostalCode(_postalCode);

    [Benchmark]
    public bool ValidatePostalCodeDntPersianUtils()
        => DNT.IranCodesUtils.IsValidIranianPostalCode(_postalCode);

    // === Shetab Card Number ===

    [Benchmark]
    public bool ValidateCardNumberIranValidator()
        => CardNumberValidator.Instance.Validate(_cardNumber).Success;

    [Benchmark]
    public bool ValidateCardNumberPersianPlus()
        => PP.IranianShetabCardExtensions.IsValidIranianShetabCardNumber(_cardNumber);

    [Benchmark]
    public bool ValidateCardNumberDntPersianUtils()
        => DNT.IranShetabUtils.IsValidIranShetabNumber(_cardNumber);

    // === IBAN (Sheba) ===

    [Benchmark]
    public bool ValidateIbanIranValidator()
        => IbanValidator.Instance.Validate(_iban).Success;

    [Benchmark]
    public bool ValidateIbanPersianPlus()
        => PP.IranianIbanExtensions.IsValidIranianIbanNumber(_iban);

    [Benchmark]
    public bool ValidateIbanDntPersianUtils()
        => DNT.IranShebaUtils.IsValidIranShebaNumber(_iban);
}

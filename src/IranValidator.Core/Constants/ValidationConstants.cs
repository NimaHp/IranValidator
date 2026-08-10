namespace IranValidator.Core.Constants;

/// <summary>
/// Constants used across validation operations.
/// </summary>
internal static class ValidationConstants
{
    // National Code
    public const int NationalCodeLength = 10;

    // Mobile
    public const int MobileLength = 11;
    public const string MobilePrefix = "09";

    // Postal Code
    public const int PostalCodeLength = 10;

    // Telephone
    public const int TelephoneLength = 11;
    public const int TelephoneLocalFirstDigitMin = '2';
    public const int TelephoneLocalFirstDigitMax = '9';

    // IBAN
    public const int IbanLength = 26;
    public const string IbanPrefix = "IR";

    // Card Number
    public const int CardNumberLength = 16;

    // Length of the bank identification number (BIN) prefix on a card.
    public const int BinLength = 6;

    // Company ID
    public const int CompanyIdLength = 11;

    // Economic Code
    public const int EconomicCodeLength = 12;

    // Passport
    public const int PassportMinLength = 8;
    public const int PassportMaxLength = 9;

    // Vehicle Plate
    public const int VehiclePlateLength = 8;

    // Maximum accepted input length, checked BEFORE normalization.
    public const int MaxInputLength = 128;
}

namespace IranValidator.Core.Results;

/// <summary>
/// Defines error codes for validation results.
/// </summary>
public enum ValidationErrorCode
{
    /// <summary>No error occurred.</summary>
    None,

    /// <summary>The input value is null or empty.</summary>
    ValueEmpty,

    /// <summary>The input length is invalid.</summary>
    InvalidLength,

    /// <summary>The checksum is invalid.</summary>
    InvalidChecksum,

    /// <summary>The format is invalid.</summary>
    InvalidFormat,

    /// <summary>The input contains invalid characters.</summary>
    InvalidCharacters,

    /// <summary>The two-digit issuance (province) code is not assigned to any Iranian province.</summary>
    InvalidProvinceCode,

    /// <summary>The three-digit bank code embedded in an Iranian IBAN is not assigned to any Iranian bank.</summary>
    InvalidBankCode,

    /// <summary>The two-digit area code is not assigned to any Iranian province.</summary>
    InvalidAreaCode,

    /// <summary>The card issuer (BIN) is not a supported Iranian bank.</summary>
    UnsupportedIssuer,

    /// <summary>The input value exceeds the maximum supported length before normalization.</summary>
    ValueTooLarge
}

namespace IranValidator.Core.Algorithms;

/// <summary>
/// Validates IBAN using MOD-97 algorithm (ISO 13616).
/// </summary>
public static class IbanAlgorithm
{
    /// <summary>
    /// Validates the IBAN checksum using MOD-97.
    /// </summary>
    /// <param name="iban">The IBAN span (country code + check digits + account number).</param>
    /// <returns>True if the IBAN is valid.</returns>
    /// <remarks>
    /// Per ISO 13616: the first 4 characters are moved to the end,
    /// letters are converted to numbers (A=10..Z=35),
    /// then MOD-97 is computed. Valid IBAN has remainder 1.
    /// The rearrangement is handled by index math — no buffer copy,
    /// no allocation, no culture-aware case conversion.
    /// </remarks>
    public static bool Validate(ReadOnlySpan<char> iban)
    {
        int len = iban.Length;
        if (len < 5 || len > 34)
            return false;

        int remainder = 0;

        // Step 1: characters 4..end come first in the rearranged string.
        for (int i = 4; i < len; i++)
        {
            remainder = ApplyDigitOrLetter(remainder, iban[i]);
            if (remainder < 0)
                return false;
        }

        // Step 2: characters 0..3 (country code + check digits) go last.
        for (int i = 0; i < 4; i++)
        {
            remainder = ApplyDigitOrLetter(remainder, iban[i]);
            if (remainder < 0)
                return false;
        }

        // Step 3: Valid IBAN has remainder 1
        return remainder == 1;
    }

    private static int ApplyDigitOrLetter(int remainder, char c)
    {
        if (c >= '0' && c <= '9')
            return (remainder * 10 + (c - '0')) % 97;

        // A=10 .. Z=35; two-digit value, tens digit first.
        int value;
        if (c >= 'A' && c <= 'Z')
        {
            value = c - 'A' + 10;
        }
        else if (c >= 'a' && c <= 'z')
        {
            value = c - 'a' + 10;
        }
        else
        {
            return -1; // Invalid character
        }

        remainder = (remainder * 10 + value / 10) % 97;
        return (remainder * 10 + value % 10) % 97;
    }
}

using IranValidator.Core.Utilities;

namespace IranValidator.Core.Algorithms;

/// <summary>
/// Validates Iranian Economic Code (کد اقتصادی) using weighted-sum checksum algorithm.
/// 12-digit code where the 12th digit is the checksum.
/// Weights: [29, 27, 23, 19, 17, 13, 7, 5, 3, 2, 1] (mod 11).
/// </summary>
internal static class EconomicCodeAlgorithm
{
    // Weights for the first 11 digits
    private static readonly int[] Weights = { 29, 27, 23, 19, 17, 13, 7, 5, 3, 2, 1 };

    /// <summary>
    /// Validates the economic code checksum.
    /// </summary>
    public static bool Validate(ReadOnlySpan<char> code)
    {
        if (code.Length != 12)
            return false;

        // All same digits is invalid
        bool allSame = true;
        for (int i = 1; i < 12; i++)
        {
            if (code[i] != code[0])
            {
                allSame = false;
                break;
            }
        }
        if (allSame)
            return false;

        // Calculate weighted sum for first 11 digits
        int sum = 0;
        for (int i = 0; i < 11; i++)
        {
            int digit = code[i].DigitToInt();
            if (digit < 0)
                return false;
            sum += digit * Weights[i];
        }

        int remainder = sum % 11;
        int checkDigit = code[11].DigitToInt();
        if (checkDigit < 0)
            return false;

        int expectedCheckDigit = remainder < 2 ? remainder : 11 - remainder;

        return checkDigit == expectedCheckDigit;
    }
}

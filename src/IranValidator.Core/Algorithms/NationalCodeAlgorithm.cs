using IranValidator.Core.Utilities;

namespace IranValidator.Core.Algorithms;

/// <summary>
/// Validates Iranian National Code using weighted-sum checksum algorithm.
/// </summary>
public static class NationalCodeAlgorithm
{
    private static readonly int[] Weights = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

    /// <summary>
    /// Validates the national code checksum.
    /// </summary>
    /// <param name="code">10-digit national code span.</param>
    /// <returns>True if the checksum is valid.</returns>
    public static bool Validate(ReadOnlySpan<char> code)
    {
        if (code.Length != 10)
            return false;

        // All same digits is invalid
        bool allSame = true;
        for (int i = 1; i < 10; i++)
        {
            if (code[i] != code[0])
            {
                allSame = false;
                break;
            }
        }
        if (allSame)
            return false;

        // Calculate weighted sum for first 9 digits
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = code[i].DigitToInt();
            if (digit < 0)
                return false;
            sum += digit * Weights[i];
        }

        int remainder = sum % 11;
        int checkDigit = code[9].DigitToInt();
        if (checkDigit < 0)
            return false;

        // Validation logic
        // If remainder < 2: check digit must equal remainder
        // If remainder >= 2: check digit must equal 11 - remainder
        int expectedCheckDigit = remainder < 2 ? remainder : 11 - remainder;

        return checkDigit == expectedCheckDigit;
    }
}

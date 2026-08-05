using IranValidator.Core.Utilities;

namespace IranValidator.Core.Algorithms;

/// <summary>
/// Generic Luhn algorithm implementation for card number validation.
/// </summary>
public static class LuhnAlgorithm
{
    /// <summary>
    /// Validates a number using the Luhn algorithm.
    /// </summary>
    /// <param name="number">The numeric string to validate.</param>
    /// <returns>True if the checksum is valid.</returns>
    public static bool Validate(ReadOnlySpan<char> number)
    {
        if (number.Length < 2)
            return false;

        int sum = 0;
        bool alternate = false;

        // Process from right to left
        for (int i = number.Length - 1; i >= 0; i--)
        {
            int digit = number[i].DigitToInt();
            if (digit < 0)
                return false;

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}

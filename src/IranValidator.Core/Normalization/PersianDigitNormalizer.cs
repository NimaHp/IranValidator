using IranValidator.Core.Utilities;

namespace IranValidator.Core.Normalization;

/// <summary>
/// Converts Persian digits (۰-۹) to Latin digits (0-9).
/// </summary>
public sealed class PersianDigitNormalizer
{
    /// <summary>
    /// Normalizes the input by replacing Persian digits with Latin digits.
    /// </summary>
    public static void Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (UnicodeHelper.IsPersianDigit(c))
                output[i] = (char)('0' + UnicodeHelper.PersianDigitToInt(c));
            else
                output[i] = c;
        }
    }

    /// <summary>
    /// Returns true if the input contains any Persian digits.
    /// </summary>
    public static bool ContainsPersianDigits(ReadOnlySpan<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (UnicodeHelper.IsPersianDigit(input[i]))
                return true;
        }
        return false;
    }
}

using IranValidator.Core.Utilities;

namespace IranValidator.Core.Normalization;

/// <summary>
/// Converts Arabic digits (٠-٩) to Latin digits (0-9).
/// </summary>
public sealed class ArabicDigitNormalizer
{
    /// <summary>
    /// Normalizes the input by replacing Arabic digits with Latin digits.
    /// </summary>
    public static void Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (UnicodeHelper.IsArabicDigit(c))
                output[i] = (char)('0' + UnicodeHelper.ArabicDigitToInt(c));
            else
                output[i] = c;
        }
    }

    /// <summary>
    /// Returns true if the input contains any Arabic digits.
    /// </summary>
    public static bool ContainsArabicDigits(ReadOnlySpan<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (UnicodeHelper.IsArabicDigit(input[i]))
                return true;
        }
        return false;
    }
}

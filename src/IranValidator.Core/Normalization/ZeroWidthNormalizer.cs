using IranValidator.Core.Utilities;

namespace IranValidator.Core.Normalization;

/// <summary>
/// Removes zero-width characters from the input.
/// </summary>
public sealed class ZeroWidthNormalizer
{
    /// <summary>
    /// Normalizes the input by removing zero-width characters.
    /// </summary>
    public static int Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        int written = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (!UnicodeHelper.IsZeroWidth(input[i]))
                output[written++] = input[i];
        }
        return written;
    }

    /// <summary>
    /// Returns true if the input contains any zero-width characters.
    /// </summary>
    public static bool ContainsZeroWidth(ReadOnlySpan<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (UnicodeHelper.IsZeroWidth(input[i]))
                return true;
        }
        return false;
    }
}

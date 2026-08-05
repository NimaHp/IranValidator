using IranValidator.Core.Utilities;

namespace IranValidator.Core.Normalization;

/// <summary>
/// Removes direction mark characters (LTR, RTL, etc.) from the input.
/// </summary>
public sealed class DirectionMarkNormalizer
{
    /// <summary>
    /// Normalizes the input by removing direction mark characters.
    /// </summary>
    public static int Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        int written = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (!UnicodeHelper.IsDirectionMark(input[i]))
                output[written++] = input[i];
        }
        return written;
    }

    /// <summary>
    /// Returns true if the input contains any direction mark characters.
    /// </summary>
    public static bool ContainsDirectionMark(ReadOnlySpan<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (UnicodeHelper.IsDirectionMark(input[i]))
                return true;
        }
        return false;
    }
}

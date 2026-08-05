namespace IranValidator.Core.Normalization;

/// <summary>
/// Removes whitespace characters from the input.
/// </summary>
public sealed class WhiteSpaceNormalizer
{
    /// <summary>
    /// Normalizes the input by removing all whitespace characters.
    /// </summary>
    public static int Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        int written = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (!char.IsWhiteSpace(input[i]))
                output[written++] = input[i];
        }
        return written;
    }

    /// <summary>
    /// Returns true if the input contains any whitespace characters.
    /// </summary>
    public static bool ContainsWhiteSpace(ReadOnlySpan<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsWhiteSpace(input[i]))
                return true;
        }
        return false;
    }
}

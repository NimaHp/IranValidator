namespace IranValidator.Core.Normalization;

/// <summary>
/// Removes dash and hyphen characters from the input.
/// </summary>
public sealed class DashNormalizer
{
    /// <summary>
    /// Normalizes the input by removing dash characters.
    /// </summary>
    public static int Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        int written = 0;
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c != '-' && c != '‐' && c != '–' && c != '—')
                output[written++] = c;
        }
        return written;
    }

    /// <summary>
    /// Returns true if the input contains any dash characters.
    /// </summary>
    public static bool ContainsDash(ReadOnlySpan<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == '-' || c == '‐' || c == '–' || c == '—')
                return true;
        }
        return false;
    }
}

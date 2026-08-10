namespace IranValidator.Core.Normalization;

/// <summary>
/// Converts Arabic letter variants to their Persian equivalents.
/// </summary>
public sealed class ArabicLetterNormalizer
{
    private const char ArabicYeh = '\u064A';  // ي
    private const char PersianYeh = '\u06CC'; // ی
    private const char ArabicKaf = '\u0643';  // ك
    private const char PersianKaf = '\u06A9'; // ک

    /// <summary>
    /// Normalizes the input by replacing Arabic letters with Persian letters.
    /// </summary>
    public static void Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == ArabicYeh)
                output[i] = PersianYeh;
            else if (c == ArabicKaf)
                output[i] = PersianKaf;
            else
                output[i] = c;
        }
    }

    /// <summary>
    /// Returns true if the input contains any Arabic letter variants.
    /// </summary>
    public static bool ContainsArabicLetter(ReadOnlySpan<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == ArabicYeh || c == ArabicKaf)
                return true;
        }
        return false;
    }
}
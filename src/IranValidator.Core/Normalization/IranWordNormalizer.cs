namespace IranValidator.Core.Normalization;

/// <summary>
/// Removes the Persian word «ایران» (the word printed on vehicle plates)
/// from the input, so the full plate spelling «۱۲ ب ۳۴۵ ایران ۶۷» is accepted
/// alongside the compact «۱۲ب۳۴۵۶۷». Arabic letter variants (ي) are already
/// converted to their Persian form by <see cref="ArabicLetterNormalizer"/>
/// earlier in the pipeline, so only the Persian «ی» is matched here.
/// </summary>
public sealed class IranWordNormalizer
{
    private const char Alef = '\u0627';       // ا
    private const char PersianYeh = '\u06CC'; // ی
    private const char Reh = '\u0631';        // ر
    private const char Nun = '\u0646';        // ن

    /// <summary>
    /// Normalizes the input by removing the word «ایران» wherever it appears.
    /// </summary>
    public static int Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        int written = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (IsIranWordAt(input, i))
            {
                i += 4; // skip the 5-character word
                continue;
            }

            output[written++] = input[i];
        }
        return written;
    }

    /// <summary>
    /// Returns true if the input contains the word «ایران».
    /// </summary>
    public static bool ContainsIranWord(ReadOnlySpan<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (IsIranWordAt(input, i))
                return true;
        }
        return false;
    }

    private static bool IsIranWordAt(ReadOnlySpan<char> input, int index)
    {
        if (index + 5 > input.Length)
            return false;

        return input[index] == Alef
            && input[index + 1] == PersianYeh
            && input[index + 2] == Reh
            && input[index + 3] == Alef
            && input[index + 4] == Nun;
    }
}

namespace IranValidator.Core.Utilities;

/// <summary>
/// Extension methods for <see cref="char"/>.
/// </summary>
internal static class CharExtensions
{
    /// <summary>Checks if a character is a Latin digit (0-9).</summary>
    public static bool IsAsciiDigit(this char c)
        => c >= '0' && c <= '9';

    /// <summary>
    /// Converts a Persian or Arabic digit to its integer value.
    /// Returns -1 if the character is not a digit.
    /// </summary>
    public static int DigitToInt(this char c)
    {
        if (c >= '0' && c <= '9')
            return c - '0';
        if (UnicodeHelper.IsPersianDigit(c))
            return UnicodeHelper.PersianDigitToInt(c);
        if (UnicodeHelper.IsArabicDigit(c))
            return UnicodeHelper.ArabicDigitToInt(c);
        return -1;
    }

    /// <summary>Checks if a character is a Persian or Arabic digit.</summary>
    public static bool IsPersianOrArabicDigit(this char c)
        => UnicodeHelper.IsPersianDigit(c) || UnicodeHelper.IsArabicDigit(c);
}

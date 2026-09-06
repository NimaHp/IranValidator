namespace IranValidator.Core.Utilities;

/// <summary>
/// Helper methods for working with Unicode characters.
/// </summary>
internal static class UnicodeHelper
{
    /// <summary>Checks if a character is a Persian digit (۰-۹).</summary>
    public static bool IsPersianDigit(char c)
        => c >= '\u06f0' && c <= '\u06f9';

    /// <summary>Checks if a character is an Arabic digit (٠-٩).</summary>
    public static bool IsArabicDigit(char c)
        => c >= '\u0660' && c <= '\u0669';

    /// <summary>Converts a Persian digit character to its integer value.</summary>
    public static int PersianDigitToInt(char c)
        => c - '\u06f0';

    /// <summary>Converts an Arabic digit character to its integer value.</summary>
    public static int ArabicDigitToInt(char c)
        => c - '\u0660';

    /// <summary>Checks if a character is a zero-width character.</summary>
    public static bool IsZeroWidth(char c)
        => c == '\u200b'  // Zero Width Space
        || c == '\u200c'  // Zero Width Non-Joiner
        || c == '\u200d'  // Zero Width Joiner
        || c == '\ufeff'; // Zero Width No-Break Space (BOM)

    /// <summary>Checks if a character is a direction mark.</summary>
    public static bool IsDirectionMark(char c)
        => c == '\u200e'  // Left-to-Right Mark
        || c == '\u200f'  // Right-to-Left Mark
        || c == '\u202a'  // Left-to-Right Embedding
        || c == '\u202b'  // Right-to-Left Embedding
        || c == '\u202c'  // Pop Directional Formatting
        || c == '\u202d'  // Left-to-Right Override
        || c == '\u202e'  // Right-to-Left Override
        || c == '\u2066'  // Left-to-Right Isolate
        || c == '\u2067'  // Right-to-Left Isolate
        || c == '\u2068'  // First Strong Isolate
        || c == '\u2069'; // Pop Directional Isolate
}

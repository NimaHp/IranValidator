namespace IranValidator.Core.Utilities;

/// <summary>
/// Helper methods for working with Unicode characters.
/// </summary>
internal static class UnicodeHelper
{
    /// <summary>Checks if a character is a Persian digit (۰-۹).</summary>
    public static bool IsPersianDigit(char c)
        => c >= '۰' && c <= '۹';

    /// <summary>Checks if a character is an Arabic digit (٠-٩).</summary>
    public static bool IsArabicDigit(char c)
        => c >= '٠' && c <= '٩';

    /// <summary>Converts a Persian digit character to its integer value.</summary>
    public static int PersianDigitToInt(char c)
        => c - '۰';

    /// <summary>Converts an Arabic digit character to its integer value.</summary>
    public static int ArabicDigitToInt(char c)
        => c - '٠';

    /// <summary>Checks if a character is a zero-width character.</summary>
    public static bool IsZeroWidth(char c)
        => c == '​'  // Zero Width Space
        || c == '‌'  // Zero Width Non-Joiner
        || c == '‍'  // Zero Width Joiner
        || c == '﻿'; // Zero Width No-Break Space (BOM)

    /// <summary>Checks if a character is a direction mark.</summary>
    public static bool IsDirectionMark(char c)
        => c == '‎'  // Left-to-Right Mark
        || c == '‏'  // Right-to-Left Mark
        || c == '‪'  // Left-to-Right Embedding
        || c == '‫'  // Right-to-Left Embedding
        || c == '‬'  // Pop Directional Formatting
        || c == '‭'  // Left-to-Right Override
        || c == '‮'  // Right-to-Left Override
        || c == '⁦'  // Left-to-Right Isolate
        || c == '⁧'  // Right-to-Left Isolate
        || c == '⁨'  // First Strong Isolate
        || c == '⁩'; // Pop Directional Isolate
}

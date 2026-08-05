using FluentAssertions;
using IranValidator.Core.Utilities;
using Xunit;

namespace IranValidator.Tests.Core;

public class SpanExtensionsTests
{
    [Fact]
    public void AsString_FromSpan_ReturnsString()
    {
        ReadOnlySpan<char> span = "hello".AsSpan();
        span.AsString().Should().Be("hello");
    }

    [Fact]
    public void AsString_EmptySpan_ReturnsEmpty()
    {
        ReadOnlySpan<char> span = ReadOnlySpan<char>.Empty;
        span.AsString().Should().BeEmpty();
    }

    [Theory]
    [InlineData("  hello  ", "hello")]
    [InlineData("hello", "hello")]
    [InlineData("   ", "")]
    [InlineData("", "")]
    [InlineData("  a b  ", "a b")]
    [InlineData("abc ", "abc")]
    [InlineData("  abc", "abc")]
    public void Trim_RemovesLeadingTrailingWhitespace(string input, string expected)
    {
        ReadOnlySpan<char> span = input.AsSpan();
        SpanExtensions.Trim(span).AsString().Should().Be(expected);
    }
}

public class CharExtensionsTests
{
    [Theory]
    [InlineData('0', true)]
    [InlineData('9', true)]
    [InlineData('5', true)]
    [InlineData('a', false)]
    [InlineData('/', false)]
    [InlineData(':', false)]
    [InlineData('\0', false)]
    public void IsAsciiDigit_DetectsCorrectly(char c, bool expected)
    {
        c.IsAsciiDigit().Should().Be(expected);
    }

    [Theory]
    [InlineData('0', 0)]
    [InlineData('9', 9)]
    [InlineData('۵', 5)]   // Persian extended
    [InlineData('٥', 5)]   // Arabic-Indic
    [InlineData('۰', 0)]   // Persian
    [InlineData('٠', 0)]   // Arabic
    [InlineData('a', -1)]
    [InlineData(' ', -1)]
    [InlineData('\0', -1)]
    public void DigitToInt_ConvertsCorrectly(char c, int expected)
    {
        c.DigitToInt().Should().Be(expected);
    }

    [Theory]
    [InlineData('۵', true)]   // Persian
    [InlineData('۰', true)]   // Persian
    [InlineData('٥', true)]   // Arabic
    [InlineData('٠', true)]   // Arabic
    [InlineData('0', false)]  // Latin
    [InlineData('a', false)]
    [InlineData(' ', false)]
    public void IsPersianOrArabicDigit_DetectsCorrectly(char c, bool expected)
    {
        c.IsPersianOrArabicDigit().Should().Be(expected);
    }
}

public class UnicodeHelperTests
{
    [Theory]
    [InlineData('\u06F0', true)]  // ۰
    [InlineData('\u06F9', true)]  // ۹
    [InlineData('\u06F5', true)]  // ۵
    [InlineData('0', false)]
    [InlineData('a', false)]
    public void IsPersianDigit_DetectsCorrectly(char c, bool expected)
    {
        UnicodeHelper.IsPersianDigit(c).Should().Be(expected);
    }

    [Theory]
    [InlineData('\u0660', true)]  // ٠
    [InlineData('\u0669', true)]  // ٩
    [InlineData('\u0665', true)]  // ٥
    [InlineData('0', false)]
    [InlineData('a', false)]
    public void IsArabicDigit_DetectsCorrectly(char c, bool expected)
    {
        UnicodeHelper.IsArabicDigit(c).Should().Be(expected);
    }

    [Theory]
    [InlineData('\u06F0', 0)]  // Persian ۰
    [InlineData('\u06F9', 9)]  // Persian ۹
    [InlineData('\u0660', 0)]  // Arabic ٠
    [InlineData('\u0669', 9)]  // Arabic ٩
    public void DigitConversion_ReturnsCorrectValue(char c, int expected)
    {
        if (UnicodeHelper.IsPersianDigit(c))
            UnicodeHelper.PersianDigitToInt(c).Should().Be(expected);
        else if (UnicodeHelper.IsArabicDigit(c))
            UnicodeHelper.ArabicDigitToInt(c).Should().Be(expected);
    }

    [Theory]
    [InlineData('\u200B', true)]  // Zero Width Space
    [InlineData('\u200C', true)]  // Zero Width Non-Joiner
    [InlineData('\u200D', true)]  // Zero Width Joiner
    [InlineData('\uFEFF', true)]  // Zero Width No-Break Space (BOM)
    [InlineData(' ', false)]
    [InlineData('-', false)]
    [InlineData('a', false)]
    public void IsZeroWidth_DetectsCorrectly(char c, bool expected)
    {
        UnicodeHelper.IsZeroWidth(c).Should().Be(expected);
    }

    [Theory]
    [InlineData('\u200E', true)]  // LRM
    [InlineData('\u200F', true)]  // RLM
    [InlineData('\u202A', true)]  // LRE
    [InlineData('\u202B', true)]  // RLE
    [InlineData('\u202C', true)]  // PDF
    [InlineData('\u202D', true)]  // LRO
    [InlineData('\u202E', true)]  // RLO
    [InlineData('\u2066', true)]  // LRI
    [InlineData('\u2067', true)]  // RLI
    [InlineData('\u2068', true)]  // FSI
    [InlineData('\u2069', true)]  // PDI
    [InlineData(' ', false)]
    [InlineData('a', false)]
    [InlineData('\u200B', false)] // Zero Width Space, not direction mark
    public void IsDirectionMark_DetectsCorrectly(char c, bool expected)
    {
        UnicodeHelper.IsDirectionMark(c).Should().Be(expected);
    }
}

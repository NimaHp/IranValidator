using FluentAssertions;
using IranValidator.Core.Normalization;
using Xunit;

namespace IranValidator.Tests.Core.Normalization;

public class NormalizerDetectionTests
{
    // === ArabicDigitNormalizer.ContainsArabicDigits ===

    [Theory]
    [InlineData("٠١٢", true)]
    [InlineData("abc", false)]
    [InlineData("123", false)]
    [InlineData("", false)]
    [InlineData("abc٩def", true)]
    [InlineData("۰۱۲", false)] // Persian, not Arabic
    public void ContainsArabicDigits_DetectsCorrectly(string input, bool expected)
    {
        ArabicDigitNormalizer.ContainsArabicDigits(input.AsSpan()).Should().Be(expected);
    }

    // === PersianDigitNormalizer.ContainsPersianDigits ===

    [Theory]
    [InlineData("۰۱۲", true)]
    [InlineData("abc", false)]
    [InlineData("123", false)]
    [InlineData("", false)]
    [InlineData("abc۹def", true)]
    [InlineData("٠١٢", false)] // Arabic, not Persian
    public void ContainsPersianDigits_DetectsCorrectly(string input, bool expected)
    {
        PersianDigitNormalizer.ContainsPersianDigits(input.AsSpan()).Should().Be(expected);
    }

    // === WhiteSpaceNormalizer.ContainsWhiteSpace ===

    [Theory]
    [InlineData(" ", true)]
    [InlineData("abc def", true)]
    [InlineData("abcdef", false)]
    [InlineData("", false)]
    [InlineData("\t", true)]
    [InlineData("\n", true)]
    [InlineData("  ", true)]
    public void ContainsWhiteSpace_DetectsCorrectly(string input, bool expected)
    {
        WhiteSpaceNormalizer.ContainsWhiteSpace(input.AsSpan()).Should().Be(expected);
    }

    // === DashNormalizer.ContainsDash ===

    [Theory]
    [InlineData("-", true)]
    [InlineData("abc-def", true)]
    [InlineData("abcdef", false)]
    [InlineData("", false)]
    [InlineData("abc‐def", true)]  // hyphen
    [InlineData("abc–def", true)]  // en-dash
    [InlineData("abc—def", true)]  // em-dash
    public void ContainsDash_DetectsCorrectly(string input, bool expected)
    {
        DashNormalizer.ContainsDash(input.AsSpan()).Should().Be(expected);
    }

    // === DirectionMarkNormalizer.ContainsDirectionMark ===

    [Theory]
    [InlineData("\u200E", true)]  // LRM
    [InlineData("\u200F", true)]  // RLM
    [InlineData("\u202A", true)]  // LRE
    [InlineData("abc\u200Edef", true)]
    [InlineData("abcdef", false)]
    [InlineData("", false)]
    public void ContainsDirectionMark_DetectsCorrectly(string input, bool expected)
    {
        DirectionMarkNormalizer.ContainsDirectionMark(input.AsSpan()).Should().Be(expected);
    }

    // === ZeroWidthNormalizer.ContainsZeroWidth ===

    [Theory]
    [InlineData("\u200B", true)]  // Zero Width Space
    [InlineData("\u200C", true)]  // Zero Width Non-Joiner
    [InlineData("\u200D", true)]  // Zero Width Joiner
    [InlineData("\uFEFF", true)]  // BOM
    [InlineData("abc\u200Bdef", true)]
    [InlineData("abcdef", false)]
    [InlineData("", false)]
    public void ContainsZeroWidth_DetectsCorrectly(string input, bool expected)
    {
        ZeroWidthNormalizer.ContainsZeroWidth(input.AsSpan()).Should().Be(expected);
    }

    // === Normalizer edge cases ===

    [Fact]
    public void ArabicDigitNormalizer_WithMixedDigits_OnlyConvertsArabic()
    {
        string input = "٠0١";
        Span<char> output = stackalloc char[input.Length];
        ArabicDigitNormalizer.Normalize(input.AsSpan(), output);
        new string(output).Should().Be("001");
    }

    [Fact]
    public void PersianDigitNormalizer_WithMixedDigits_OnlyConvertsPersian()
    {
        string input = "۰0۱";
        Span<char> output = stackalloc char[input.Length];
        PersianDigitNormalizer.Normalize(input.AsSpan(), output);
        new string(output).Should().Be("001");
    }

    [Fact]
    public void DirectionMarkNormalizer_RemovesAllDirectionMarks()
    {
        Span<char> output = stackalloc char[10];
        int len = DirectionMarkNormalizer.Normalize("a\u200Eb\u200Fc".AsSpan(), output);
        output.Slice(0, len).ToString().Should().Be("abc");
    }

    [Fact]
    public void WhiteSpaceNormalizer_RemovesAllWhitespace()
    {
        Span<char> output = stackalloc char[10];
        int len = WhiteSpaceNormalizer.Normalize("a b\tc\nd".AsSpan(), output);
        output.Slice(0, len).ToString().Should().Be("abcd");
    }

    [Fact]
    public void DashNormalizer_RemovesAllDashVariants()
    {
        Span<char> output = stackalloc char[10];
        int len = DashNormalizer.Normalize("a-b‐c–d—e".AsSpan(), output);
        output.Slice(0, len).ToString().Should().Be("abcde");
    }

    [Fact]
    public void ZeroWidthNormalizer_RemovesAllZeroWidthChars()
    {
        Span<char> output = stackalloc char[10];
        int len = ZeroWidthNormalizer.Normalize("a\u200Bb\u200Cc".AsSpan(), output);
        output.Slice(0, len).ToString().Should().Be("abc");
    }

    [Fact]
    public void CompositeNormalizer_HandlesAllVariantsTogether()
    {
        var normalizer = new CompositeNormalizer();
        var result = normalizer.Normalize("\u200E۰۹۱۲-۱۲۳ ۴۵۶۷\u200F".AsSpan());
        result.Should().Be("09121234567");
    }
}

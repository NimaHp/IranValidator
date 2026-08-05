using FluentAssertions;
using IranValidator.Core.Normalization;
using Xunit;

namespace IranValidator.Tests.Core.Normalization;

public class PersianDigitNormalizerTests
{
    [Theory]
    [InlineData("۰۹۱۲", "0912")]
    [InlineData("۱۲۳۴۵", "12345")]
    [InlineData("۹۹۹", "999")]
    public void Normalize_PersianDigits_ConvertsCorrectly(string input, string expected)
    {
        Span<char> output = stackalloc char[input.Length];
        PersianDigitNormalizer.Normalize(input.AsSpan(), output);
        output.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("0912", false)]
    [InlineData("۰۹۱۲", true)]
    public void ContainsPersianDigits_DetectsCorrectly(string input, bool expected)
    {
        PersianDigitNormalizer.ContainsPersianDigits(input.AsSpan()).Should().Be(expected);
    }
}

public class ArabicDigitNormalizerTests
{
    [Theory]
    [InlineData("٠٩١٢", "0912")]
    [InlineData("١٢٣٤٥", "12345")]
    public void Normalize_ArabicDigits_ConvertsCorrectly(string input, string expected)
    {
        Span<char> output = stackalloc char[input.Length];
        ArabicDigitNormalizer.Normalize(input.AsSpan(), output);
        output.ToString().Should().Be(expected);
    }
}

public class WhiteSpaceNormalizerTests
{
    [Theory]
    [InlineData("0912 123 4567", "09121234567")]
    [InlineData("0912  123", "0912123")]
    [InlineData(" 0912 ", "0912")]
    public void Normalize_RemovesWhitespace(string input, string expected)
    {
        Span<char> output = stackalloc char[input.Length];
        int length = WhiteSpaceNormalizer.Normalize(input.AsSpan(), output);
        output.Slice(0, length).ToString().Should().Be(expected);
    }
}

public class DashNormalizerTests
{
    [Theory]
    [InlineData("0912-123-4567", "09121234567")]
    [InlineData("0912–123", "0912123")]
    public void Normalize_RemovesDashes(string input, string expected)
    {
        Span<char> output = stackalloc char[input.Length];
        int length = DashNormalizer.Normalize(input.AsSpan(), output);
        output.Slice(0, length).ToString().Should().Be(expected);
    }
}

public class CompositeNormalizerTests
{
    private readonly CompositeNormalizer _normalizer = new();

    [Theory]
    [InlineData("۰۹۱۲-۱۲۳ ۴۵۶۷", "09121234567")]
    [InlineData("٠٩١٢ ١٢٣", "0912123")]
    [InlineData("۰۹۱۲​۱۲۳", "0912123")]
    public void Normalize_ComplexInput_ConvertsCorrectly(string input, string expected)
    {
        var result = _normalizer.Normalize(input.AsSpan());
        result.Should().Be(expected);
    }

    [Fact]
    public void Normalize_EmptyInput_ReturnsEmpty()
    {
        var result = _normalizer.Normalize(ReadOnlySpan<char>.Empty);
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("09121234567")]
    [InlineData("P12345678")]
    [InlineData("0010350829")]
    [InlineData("IR820540102680020817909002")]
    [InlineData("12B34567")]
    public void Normalize_WithOriginal_ReturnsSameReferenceWhenNoNormalizationNeeded(string input)
    {
        var result = _normalizer.Normalize(input.AsSpan(), input);
        result.Should().BeSameAs(input);
    }

    [Theory]
    [InlineData("۰۹۱۲۱۲۳۴۵۶۷")]   // Persian digits
    [InlineData("٠٩١٢١٢٣٤٥٦٧")]   // Arabic digits
    [InlineData("0912 1234567")]  // whitespace
    [InlineData("0912-1234567")]  // ASCII dash
    [InlineData("0912–1234567")]  // en dash
    [InlineData("09121234567\n")] // newline
    public void Normalize_WithOriginal_ReturnsNewStringWhenNormalizationNeeded(string input)
    {
        var result = _normalizer.Normalize(input.AsSpan(), input);
        result.Should().NotBeSameAs(input);
        result.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("09121234567", false)]
    [InlineData("IR820540102680020817909002", false)]
    [InlineData("12B34567", false)]
    [InlineData("0010350829", false)]
    [InlineData("0912 1234567", true)]
    [InlineData("0912-1234567", true)]
    [InlineData("۰۹۱۲۱۲۳۴۵۶۷", true)]
    [InlineData("٠٩١٢١٢٣٤٥٦٧", true)]
    [InlineData("0912\t1234567", true)]
    public void NeedsNormalization_DetectsCorrectly(string input, bool expected)
    {
        CompositeNormalizer.NeedsNormalization(input.AsSpan()).Should().Be(expected);
    }

    [Fact]
    public void Normalize_WithNullOriginal_FallsBackToSpanOverload()
    {
        var result = _normalizer.Normalize("0912 1234567".AsSpan(), null);
        result.Should().Be("09121234567");
    }
}

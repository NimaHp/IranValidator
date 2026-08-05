namespace IranValidator.Core.Utilities;

/// <summary>
/// Extension methods for <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
/// </summary>
internal static class SpanExtensions
{
    /// <summary>
    /// Copies a span to a new string allocation.
    /// </summary>
    public static string AsString(this ReadOnlySpan<char> span)
#if NETSTANDARD2_0
        => span.ToString();
#else
        => new string(span);
#endif

    /// <summary>
    /// Trims whitespace from both ends of a span.
    /// </summary>
    public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span)
    {
        int start = 0;
        while (start < span.Length && char.IsWhiteSpace(span[start]))
            start++;

        int end = span.Length - 1;
        while (end >= start && char.IsWhiteSpace(span[end]))
            end--;

        return span.Slice(start, end - start + 1);
    }
}

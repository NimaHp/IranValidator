namespace IranValidator.Core.Normalization;

/// <summary>
/// Composite normalizer that applies multiple normalizers in sequence.
/// </summary>
public sealed class CompositeNormalizer
{
    /// <summary>
    /// Applies all normalizers in sequence and returns the normalized string.
    /// </summary>
    public string Normalize(ReadOnlySpan<char> input)
    {
        if (input.IsEmpty)
            return string.Empty;

        // Allocate buffer large enough for worst case
        int maxLength = input.Length;
        Span<char> buffer = stackalloc char[maxLength];
        Span<char> temp = stackalloc char[maxLength];

        // Copy input to buffer
        input.CopyTo(buffer);
        int currentLength = input.Length;

        // Apply each normalizer
        currentLength = WhiteSpaceNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

        currentLength = DashNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

        currentLength = ZeroWidthNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

        currentLength = DirectionMarkNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

        PersianDigitNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

        ArabicDigitNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

#if NETSTANDARD2_0
        return temp.Slice(0, currentLength).ToString();
#else
        return new string(temp.Slice(0, currentLength));
#endif
    }

    /// <summary>
    /// Normalizes the input, returning the original string reference when no
    /// normalization is required (zero-allocation fast path). This mirrors how
    /// <see cref="string.PadLeft(int)"/>/<see cref="string.Replace(string?, string?)"/>
    /// return the current instance when no work is needed.
    /// </summary>
    /// <param name="input">The input to normalize.</param>
    /// <param name="original">
    /// The original string backing <paramref name="input"/>, or <see langword="null"/>
    /// when the input is a span without a backing string (e.g. sliced input).
    /// </param>
    public string Normalize(ReadOnlySpan<char> input, string? original)
        => original is not null && !NeedsNormalization(input) ? original : Normalize(input);

    /// <summary>
    /// Returns true when the input contains any character the normalization
    /// pipeline would remove or convert. For plain ASCII digit/letter input
    /// (the overwhelmingly common case) this is a single cheap scan.
    /// </summary>
    public static bool NeedsNormalization(ReadOnlySpan<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            // c < 0x21: control chars + space (whitespace) — always re-checked
            // c == '-': ASCII dash
            // c >= 0x7F: DEL, Persian/Arabic digits, zero-width chars,
            //            direction marks, en/em dashes, NBSP, ...
            if (c < 0x21 || c == '-' || c >= 0x7F)
                return true;
        }

        return false;
    }
}

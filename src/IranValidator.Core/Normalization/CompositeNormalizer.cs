using System.Buffers;

namespace IranValidator.Core.Normalization;

/// <summary>
/// Composite normalizer that applies multiple normalizers in sequence.
/// </summary>
public sealed class CompositeNormalizer
{
    /// <summary>
    /// Inputs of this length or less use stack-allocated scratch buffers;
    /// longer inputs rent pooled heap buffers so oversized input can never
    /// exhaust the thread stack (StackOverflowException is uncatchable).
    /// </summary>
    private const int StackallocThreshold = 1024;

    /// <summary>
    /// Applies all normalizers in sequence and returns the normalized string.
    /// </summary>
    public string Normalize(ReadOnlySpan<char> input)
    {
        if (input.IsEmpty)
            return string.Empty;

        // Small inputs use stack-allocated scratch buffers; larger inputs rent
        // pooled heap buffers instead of raw stackalloc, which would crash the
        // process (StackOverflowException is uncatchable) on oversized input.
        if (input.Length <= StackallocThreshold)
        {
            Span<char> buffer = stackalloc char[input.Length];
            Span<char> temp = stackalloc char[input.Length];
            return NormalizeCore(buffer, temp, input);
        }

        char[] bufferPool = ArrayPool<char>.Shared.Rent(input.Length);
        char[] tempPool = ArrayPool<char>.Shared.Rent(input.Length);
        try
        {
            return NormalizeCore(bufferPool.AsSpan(0, input.Length), tempPool.AsSpan(0, input.Length), input);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(bufferPool);
            ArrayPool<char>.Shared.Return(tempPool);
        }
    }

    private static string NormalizeCore(Span<char> buffer, Span<char> temp, ReadOnlySpan<char> input)
    {
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

        // Arabic letter variants (ي، ك) are converted to their Persian
        // equivalents (ی، ک) first, so the plate letter and the «ایران» word
        // are matched as one regardless of how the user's keyboard typed them.
        ArabicLetterNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

        // The word «ایران» printed on vehicle plates is stripped here so the
        // full spelling «۱۲ ب ۳۴۵ ایران ۶۷» normalizes to «۱۲ب۳۴۵۶۷». This only
        // ever matches plate inputs; no other Iranian identifier contains it.
        currentLength = IranWordNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

        PersianDigitNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

        ArabicDigitNormalizer.Normalize(buffer.Slice(0, currentLength), temp);
        temp.Slice(0, currentLength).CopyTo(buffer);

        // NOTE: 'temp' holds the final content — the last CopyTo above copies
        // from 'temp' to 'buffer' and does not modify 'temp'.
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

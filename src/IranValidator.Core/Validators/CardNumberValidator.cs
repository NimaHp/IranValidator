using IranValidator.Core.Algorithms;
using IranValidator.Core.Constants;
using IranValidator.Core.Normalization;
using IranValidator.Core.Results;
using IranValidator.Core.Utilities;

namespace IranValidator.Core.Validators;

/// <summary>
/// Validates Iranian bank card numbers (شماره کارت) using the Luhn algorithm
/// plus a strict issuer check: the 6-digit BIN must belong to an Iranian bank
/// (see <see cref="IranianBankBins"/>). Iranian cards are exactly 16 digits.
/// </summary>
public sealed class CardNumberValidator : IStringValidator
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static CardNumberValidator Instance { get; } = new();

    private static readonly CompositeNormalizer Normalizer = new();

    private CardNumberValidator() { }

    /// <inheritdoc/>
    public ValidationResult Validate(string value)
    {
        if (value is null || value.Length == 0)
            return ValidationResult.Error(ValidationErrorCode.ValueEmpty);

        return ValidateCore(value.AsSpan(), value);
    }

    /// <inheritdoc/>
    public ValidationResult Validate(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return ValidationResult.Error(ValidationErrorCode.ValueEmpty);

        return ValidateCore(value, null);
    }

    private static ValidationResult ValidateCore(ReadOnlySpan<char> value, string? original)
    {

        // Normalize input
        string normalized = Normalizer.Normalize(value, original);

        // Whitespace-only input normalizes to empty — report it as an empty value.
        if (normalized.Length == 0)
            return ValidationResult.Error(ValidationErrorCode.ValueEmpty);
        ReadOnlySpan<char> card = normalized.AsSpan();

        // Must be exactly 16 digits
        if (card.Length != ValidationConstants.CardNumberLength)
            return ValidationResult.Error(ValidationErrorCode.InvalidLength);

        // Validate all characters are digits
        for (int i = 0; i < card.Length; i++)
        {
            if (!card[i].IsAsciiDigit())
                return ValidationResult.Error(ValidationErrorCode.InvalidCharacters);
        }

        // Delegate to Luhn algorithm
        if (!LuhnAlgorithm.Validate(card))
            return ValidationResult.Error(ValidationErrorCode.InvalidChecksum);

        // Strict issuer check: the 6-digit BIN must belong to an Iranian bank.
        if (!IranianBankBins.Contains(ParseBin(card)))
            return ValidationResult.Error(ValidationErrorCode.UnsupportedIssuer);

        return ValidationResult.Ok(normalized);
    }

    private static int ParseBin(ReadOnlySpan<char> card)
    {
        int bin = 0;
        for (int i = 0; i < ValidationConstants.BinLength; i++)
            bin = bin * 10 + (card[i] - '0');

        return bin;
    }
}

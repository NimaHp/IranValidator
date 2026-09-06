using IranValidator.Core.Utilities;

namespace IranValidator.Core.Algorithms;

/// <summary>
/// Validates Iranian Legal Entity National ID (شناسه ملی اشخاص حقوقی — ۱۱ رقم).
/// Source: ilenc.ir / https://excelengineer.ir/excel_id_code/ (sheet "شناسه ملی حقوقی", B5)
/// and the attached file "کد-ملی-و-شناسه-ملی.xlsx".
/// Weights for the first 10 digits: [29, 27, 23, 19, 17, 29, 27, 23, 19, 17] (sum = 230).
/// Formula: rem = ( Σ d[i]*w[i] + (d10+2)*Σw ) % 11 ; valid when (rem==0||rem==10) ? check==0 : rem==check.
/// </summary>
internal static class CompanyIdAlgorithm
{
    private static readonly int[] Weights = { 29, 27, 23, 19, 17, 29, 27, 23, 19, 17 };
    private const int WeightsSum = 230;

    /// <summary>
    /// Validates the company ID checksum.
    /// </summary>
    /// <param name="code">11-digit company ID span (already verified as 11 ASCII digits).</param>
    /// <returns>True if the checksum is valid and not all digits are identical.</returns>
    public static bool Validate(ReadOnlySpan<char> code)
    {
        if (code.Length != 11)
            return false;

        // All same digits is invalid (e.g. 11111111111)
        bool allSame = true;
        for (int i = 1; i < 11; i++)
        {
            if (code[i] != code[0])
            {
                allSame = false;
                break;
            }
        }
        if (allSame)
            return false;

        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            int digit = code[i].DigitToInt();
            if (digit < 0)
                return false;
            sum += digit * Weights[i];
        }

        int d10 = code[9].DigitToInt();
        if (d10 < 0)
            return false;
        int remainder = (sum + (d10 + 2) * WeightsSum) % 11;
        // Excel maps remainder 0 and 10 both to expected check 0.
        int expected = (remainder == 0 || remainder == 10) ? 0 : remainder;
        int checkDigit = code[10].DigitToInt();
        if (checkDigit < 0)
            return false;

        return checkDigit == expected;
    }
}

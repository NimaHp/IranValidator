namespace IranValidator.Core.Constants;

/// <summary>
/// Two-digit issuance (province) codes used on Iranian vehicle plates
/// (پلاک خودرو). The last two characters of a standard plate identify the
/// issuing province; together with the middle letter they narrow it down to
/// the issuing county. <see cref="Contains"/> is the strict assignment check
/// used by <see cref="Validators.VehiclePlateValidator"/>.
/// </summary>
/// <remarks>
/// Source: Wikipedia — «پلاک وسایل نقلیه در ایران», section «شهرستان‌ها و کد
/// پلاک سراسر کشور» (cross-checked 2026-08-01). 87 codes are currently
/// assigned; 39, 80 and 90 are unused/reserved.
/// Cross-check against persian-tools (JS) <c>numberplate</c> module: it maps 45
/// to Yazd, which is wrong — 45, 65, 75 belong to Kerman (کرمان); Yazd is
/// 54, 64. persian-tools also omits Tehran's 50/60/70 and Yazd's 54. This
/// table follows Wikipedia.
/// Several codes are shared between provinces — 21, 30, 38, 78 (Tehran/Alborz)
/// and 32, 42 (the three Khorasan provinces) — and the middle letter is what
/// disambiguates the actual issuing county; such shared codes are listed once
/// with a comment. Codes remain valid even when future plate allocation moves
/// from county-level to province-level series.
/// </remarks>
public static class IranianProvinceCodes
{
    // Grouped by province for maintainability, following IranianBankBins.cs.
    // All values are the two-digit codes printed in the square box at the end
    // of the plate.
    private static readonly HashSet<int> Codes = new()
    {
        // تهران (19): 11 21 22 33 38 44 55 66 77 78 88 99 10 20 30 40 50 60 70
        11, 21, 22, 33, 38, 44, 55, 66, 77, 78, 88, 99, 10, 20, 30, 40, 50, 60, 70,

        // البرز (5) — shares 21, 30, 38, 78 with تهران
        68,

        // خراسان رضوی (5) — shares 32, 42 with خراسان شمالی/جنوبی
        12, 32, 36, 42, 74,

        // اصفهان (5)
        13, 23, 43, 53, 67,

        // فارس (4)
        63, 73, 83, 93,

        // مازندران (4)
        62, 72, 82, 92,

        // خوزستان (3)
        14, 24, 34,

        // آذربایجان شرقی (3)
        15, 25, 35,

        // آذربایجان غربی (3)
        17, 27, 37,

        // کرمان (3)
        45, 65, 75,

        // گیلان (3)
        46, 56, 76,

        // خراسان شمالی (3) — shares 32, 42
        26,

        // خراسان جنوبی (3) — shares 32, 42
        52,

        // همدان (2)
        18, 28,

        // کرمانشاه (2)
        19, 29,

        // لرستان (2)
        31, 41,

        // مرکزی (2)
        47, 57,

        // بوشهر (2)
        48, 58,

        // یزد (2)
        54, 64,

        // کردستان (2)
        51, 61,

        // گلستان (2)
        59, 69,

        // چهارمحال و بختیاری (2)
        71, 81,

        // قزوین (2)
        79, 89,

        // هرمزگان (2)
        84, 94,

        // سیستان و بلوچستان (2)
        85, 95,

        // سمنان (2)
        86, 96,

        // زنجان (2)
        87, 97,

        // قم (1)
        16,

        // اردبیل (1)
        91,

        // کهگیلویه و بویراحمد (1)
        49,

        // ایلام (1)
        98,
    };

    /// <summary>
    /// Returns <c>true</c> when <paramref name="code"/> is an assigned
    /// two-digit plate issuance code; otherwise <c>false</c>.
    /// </summary>
    public static bool Contains(int code) => Codes.Contains(code);
}

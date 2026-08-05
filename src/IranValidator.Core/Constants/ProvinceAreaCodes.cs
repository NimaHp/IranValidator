namespace IranValidator.Core.Constants;

/// <summary>
/// Assigned Iranian landline area codes — one 2-digit code per province.
///
/// Single source of truth for the landline area-code list. To update
/// (e.g. when a code changes): edit <see cref="Valid"/>, KEEP IT SORTED,
/// and update the "checked" date in the sources comment. The test
/// <c>ProvinceAreaCodes_IsSortedUniqueAndWithinRange</c> guards sort order,
/// uniqueness and range, so an unsorted update fails CI.
///
/// Since the nationwide area-code unification plan (طرح هم‌کدسازی, completed
/// 2014) every province has exactly one 2-digit area code and every local
/// number is 8 digits: 0 + XX + 8 digits = 11 digits total. County-level
/// codes no longer exist — they were unified into the provincial code.
///
/// Sources (union, checked 2026-08-03 — both lists identical):
///   - Persian Wikipedia, «شماره‌های تلفن در ایران» — جدول کد استان‌ها پس از هم‌کدسازی.
///   - English Wikipedia, "Telephone numbers in Iran" — "Area codes" table.
/// </summary>
internal static class ProvinceAreaCodes
{
    /// <summary>Valid 2-digit province area codes as numbers (e.g. 21 = 021 Tehran). Must stay sorted.</summary>
    public static readonly ushort[] Valid =
    {
        11, 13, 17,                                     // Mazandaran, Gilan, Golestan
        21, 23, 24, 25, 26, 28,                         // Tehran, Semnan, Zanjan, Qom, Alborz, Qazvin
        31, 34, 35, 38,                                 // Isfahan, Kerman, Yazd, Chaharmahal & Bakhtiari
        41, 44, 45,                                     // East Azerbaijan, West Azerbaijan, Ardabil
        51, 54, 56, 58,                                 // Razavi Khorasan, Sistan & Baluchestan, South Khorasan, North Khorasan
        61, 66,                                         // Khuzestan, Lorestan
        71, 74, 76, 77,                                 // Fars, Kohgiluyeh & Boyer-Ahmad, Hormozgan, Bushehr
        81, 83, 84, 86, 87,                             // Hamadan, Kermanshah, Ilam, Markazi, Kurdistan
    };

    /// <summary>Checks whether the given 2-digit area code (e.g. 21 for 021…) is assigned to a province.</summary>
    public static bool Contains(ushort code) => Array.BinarySearch(Valid, code) >= 0;
}

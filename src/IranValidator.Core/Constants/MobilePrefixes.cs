namespace IranValidator.Core.Constants;

/// <summary>
/// Assigned Iranian mobile operator prefixes (09XX).
///
/// Single source of truth for the mobile prefix list. To update (e.g. when a new
/// prefix is assigned): add/remove entries below, KEEP THE ARRAY SORTED, and update
/// the "Checked" date. The test <c>MobilePrefixes_IsSortedAndWithinRange</c> guards
/// sort order and range, so an unsorted update fails CI.
///
/// Sources (union, checked 2026-08-03):
///   - Persian Wikipedia, "شماره‌های تلفن در ایران": 0900-0905, 0910-0919,
///     0920-0923, 0930-0939, 0941-0942, 0990-0996, 0998-0999.
///   - persian-tools (github.com/persian-tools/persian-tools, phoneNumber module):
///     adds 0924. The two sources disagree only on 0924 (persian-tools) and 0942
///     (Wikipedia); the union keeps both to avoid rejecting assigned numbers.
///   - Persian Wikipedia, "فهرست اپراتورهای تلفن همراه در ایران" (MVNO list):
///     لوتوس تل 09990، آپتل 09991/09993، فناپ موبایل 09992، هایوب 0995،
///     شاتل موبایل 09981/09982، همراه هوشمند آینده 09989، آرین تل 09998.
///   - Persian Wikipedia, "سامانتل": پیش‌شماره MVNO سامانتل 09999 (دائمی و
///     اعتباری)؛ برند باماتل 09995 (اعتباری).
///   Dual assignments kept per the same union policy: 0995 = هایوب (MVNO list) /
///   باماتل (سامانتل article); 09998 = آرین تل (MVNO list) / سامانتل اعتباری
///   (سامانتل website). All covered by the 3-digit prefixes below.
/// </summary>
internal static class MobilePrefixes
{
    /// <summary>Valid 09XX prefixes as numbers (e.g. 912 = 0912). Must stay sorted.</summary>
    public static readonly ushort[] Valid =
    {
        900, 901, 902, 903, 904, 905,          // Irancell / AsiaTech
        910, 911, 912, 913, 914, 915, 916, 917, 918, 919, // MCI
        920, 921, 922, 923, 924,               // Rightel
        930, 931, 932, 933, 934, 935, 936, 937, 938, 939, // Irancell + MVNOs (Espadan, Taliya, Kish)
        941, 942,                              // Irancell TD-LTE
        // 099x MVNO range — sub-prefix detail (first 3 digits after 0):
        //  995: 0995 هایوب / باماتل (سامانتل، اعتباری)
        //  998: 09981/82 شاتل موبایل، 09989 همراه هوشمند آینده، 09998 آرین تل + سامانتل اعتباری
        //  999: 09990 لوتوس تل، 09991/93 آپتل، 09992 فناپ موبایل، 09999 سامانتل (دائمی/اعتباری)
        990, 991, 992, 993, 994, 995, 996,     // MVNOs (see sub-prefix detail)
        998, 999,                              // MVNOs (see sub-prefix detail)
    };

    /// <summary>Checks whether the given 4-digit prefix (e.g. 912 for 0912…) is assigned.</summary>
    public static bool Contains(int prefix) => Array.BinarySearch(Valid, (ushort)prefix) >= 0;
}

namespace IranValidator.Core.Constants;

/// <summary>
/// Six-digit Bank Identification Numbers (BINs) assigned to Iranian banks on the
/// Shetab card network. A valid Iranian bank card must start with one of these
/// prefixes; <see cref="Contains"/> is the strict issuer check used by
/// <see cref="Validators.CardNumberValidator"/>.
/// </summary>
/// <remarks>
/// Sources (cross-checked on 2026-08-01):
/// <list type="bullet">
/// <item>DNTPersianUtils.Core — <c>IranianBankCardProviders</c> (40 BINs)</item>
/// <item>Persian.Plus — <c>IranBankConstants.BankCardBins</c></item>
/// <item>persian-tools (JS) — <c>getBankNameFromCardNumber/banksCode</c></item>
/// </list>
/// Disputed prefixes were resolved by majority vote; single-source entries are
/// flagged inline. BINs are kept regardless of issuer mergers or dissolutions
/// because the cards remain in circulation; the inline comments note the
/// current legal issuer where known. Merger facts verified 2026-08-01 against
/// Wikipedia/CBI notices: Ghavamin (639599), Mehr Eqtesad (504944, 639370),
/// Kosar (505801) and Hekmat (636949) merged into Bank Sepah per the CBI
/// notice of 1397-12-11 (2019-03-02); Ayandeh (636214) was dissolved and
/// merged into Bank Melli on 2025-10-23; Tose'e Credit Institution (628157)
/// was dissolved by the CBI in 2023-09. The official list published by Shetab
/// (shetab.ir) could not be reached from CI; revisit when it becomes available.
/// </remarks>
public static class IranianBankBins
{
    // Organized by issuer for maintainability. All values are 6-digit BINs.
    private static readonly HashSet<int> Bins = new()
    {
        // Bank Melli Iran — 603799 (old); 455379, 170019 (single-source)
        603799, 455379, 170019,

        // Bank Saderat Iran — 603769 (old); 440796, 903769 (single-source)
        603769, 440796, 903769,

        // Bank Mellat
        610433, 991975,

        // Bank Sepah
        589210,

        // Bank Tejarat
        627353, 585983,

        // Bank Keshavarzi
        603770, 639217,

        // Bank Maskan
        628023,

        // Post Bank
        627760,

        // Bank of Industry and Mine (صنعت و معدن)
        627961,

        // Export Development Bank (توسعه صادرات); 207177 single-source
        627648, 207177,

        // Bank Refah
        589463,

        // Central Bank of Iran; 636797 single-source (persian-tools)
        636795, 636797,

        // Bank Pasargad — 502229 (old); 639347 (new)
        502229, 639347,

        // Bank Parsian — 622106 (old); 627884, 639194 (new)
        622106, 627884, 639194,

        // Bank Karafarin
        627488, 502910,

        // Bank Saman
        621986,

        // Bank Sarmayeh
        639607,

        // Bank Sina
        639346,

        // Bank Shahr
        504706, 502806,

        // Cooperative Development Bank (توسعه تعاون)
        502908,

        // Eghtesad Novin Bank
        627412,

        // Bank Ansar
        627381,

        // Iran Zamin Bank
        505785,

        // Bank Day
        502938,

        // Tourism Bank (گردشگری); 505426 single-source (persian-tools)
        505416, 505426,

        // Resalat Qarz al-Hasaneh Bank
        504172,

        // Middle East Bank (خاورمیانه); 505809 (DNTPersianUtils), 585647 (Persian.Plus)
        585947, 585647, 505809,

        // Ghavamin Bank (merged into Sepah per CBI 1397-12-11)
        639599,

        // Kosar Credit Institution (merged into Sepah per CBI 1397-12-11)
        505801,

        // Mehr Eqtesad Bank (merged into Sepah per CBI 1397-12-11);
        // 639370 also attributed to "قرض الحسنه مهر" by persian-tools
        504944, 639370,

        // Mehr Iran Bank
        606373,

        // Hekmat Iranian Bank (merged into Sepah per CBI 1397-12-11)
        636949,

        // Noor Credit Institution
        507677,

        // Mellal Credit Institution; 606256 (DNT uses the pre-1395 name
        // "Asgariyeh"; renamed to Mellal in 1395)
        606256,

        // Tose'e Credit Institution (dissolved by CBI 2023-09; legacy BIN)
        628157,

        // Iran-Venezuela Joint Bank (single-source; status unverified)
        581874,

        // Ayandeh Bank (dissolved into Bank Melli on 2025-10-23;
        // its cards are now serviced by Bank Melli)
        636214,
    };

    /// <summary>
    /// Returns <c>true</c> when the 6-digit prefix belongs to an Iranian bank.
    /// </summary>
    public static bool Contains(int bin) => Bins.Contains(bin);
}

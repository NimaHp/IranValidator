namespace IranValidator.Core.Constants;

/// <summary>
/// Three-digit bank codes used inside Iranian IBAN (شبا) numbers. In an IBAN
/// of the form IRkk-BBB-AAAAAAAAAAAAAAAAAAA the code occupies positions 5-7
/// (1-based); <see cref="Contains"/> is the strict issuer check used by
/// <see cref="Validators.IbanValidator"/>.
/// </summary>
/// <remarks>
/// Sources (cross-checked 2026-08-01):
/// <list type="bullet">
/// <item>almico.ir/blog/sheba — «رقم 4 و 5 شبا» is the LAST TWO digits of the
/// 3-digit code (the 4th and 5th digits of the 24-digit numeric part); the
/// full code is that suffix zero-padded to 3 digits (صنعت و معدن «11» → 011,
/// ملت «12» → 012, ملی «17» → 017). All 32 rows match the other sources.</item>
/// <item>persian-tools (JS) — <c>sheba/codes.skip.ts</c> map (38 codes); adds
/// 010 (Central Bank), 051 (Tose'e), 073 (Kosar), 078, 090 (Mehr Iran repeat)
/// and 095 (Iran-Venezuela) missing from almico.</item>
/// </list>
/// Disputed assignment: almico and pishkhanak.com assign 080 to Middle East
/// Bank (خاورمیانه) while persian-tools assigns 078 to it and 080 to Noor
/// credit institute. Both 078 and 080 are kept so no real account is rejected;
/// the authoritative Shetab list could not be reached from CI — revisit when
/// available. Codes are kept regardless of issuer mergers or dissolutions
/// (Ghavamin 052, Mehr Eqtesad 079, Mehr Iran 060/090 merged into Sepah;
/// Ayandeh 062 into Melli) because existing شبا accounts remain valid.
/// </remarks>
public static class IranianShebaBankCodes
{
    // Organized by issuer for maintainability. All values are 3-digit codes.
    private static readonly HashSet<int> Codes = new()
    {
        // Government / state banks (010-022)
        010, // Central Bank of Iran (بانک مرکزی) — persian-tools only
        011, // Bank of Industry and Mine (صنعت و معدن)
        012, // Bank Mellat (ملت)
        013, // Bank Refah (رفاه کارگران)
        014, // Bank Maskan (مسکن)
        015, // Bank Sepah (سپه)
        016, // Bank Keshavarzi (کشاورزی)
        017, // Bank Melli (ملی)
        018, // Bank Tejarat (تجارت)
        019, // Bank Saderat (صادرات)
        020, // Export Development Bank (توسعه صادرات)
        021, // Post Bank (پست بانک)
        022, // Tose'e Ta'avon Bank (توسعه تعاون)

        // Credit institutions & private banks (051-080)
        051, // Tose'e Credit Institution (موسسه اعتباری توسعه) — persian-tools only
        052, // Ghavamin Bank (قوامین) — merged into Sepah
        053, // Karafarin Bank (کارآفرین)
        054, // Parsian Bank (پارسیان)
        055, // Eghtesad Novin Bank (اقتصاد نوین)
        056, // Saman Bank (سامان)
        057, // Pasargad Bank (پاسارگاد)
        058, // Sarmayeh Bank (سرمایه)
        059, // Sina Bank (سینا)
        060, // Mehr Iran Bank (مهر ایران) — merged into Sepah
        061, // Shahr Bank (شهر)
        062, // Ayandeh Bank (آینده) — merged into Melli
        063, // Ansar Bank (انصار)
        064, // Gardeshgari Bank (گردشگری)
        065, // Hekmat Iranian Bank (حکمت ایرانیان) — merged into Sepah
        066, // Dey Bank (دی)
        069, // Iran Zamin Bank (ایران زمین)
        070, // Resalat Bank (رسالت)
        073, // Kosar Credit Institute (موسسه اعتباری کوثر) — persian-tools only
        075, // Melal Credit Institute (موسسه اعتباری ملل)
        078, // Middle East Bank (خاورمیانه) — persian-tools; disputed, see remarks
        079, // Mehr Eqtesad Bank (مهر اقتصاد) — merged into Sepah
        080, // Middle East Bank / Noor — disputed, see remarks

        // Minor / single-source (090-095)
        090, // Mehr Iran Bank repeat (مهر ایران) — persian-tools only
        095, // Iran-Venezuela Bank (ایران و ونزوئلا) — persian-tools only
    };

    /// <summary>
    /// Returns <c>true</c> when <paramref name="code"/> is a known Iranian
    /// IBAN bank code; otherwise <c>false</c>.
    /// </summary>
    public static bool Contains(int code) => Codes.Contains(code);
}

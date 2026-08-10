using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class VehiclePlateValidatorTests
{
    private readonly VehiclePlateValidator _sut = VehiclePlateValidator.Instance;

    [Theory]
    [InlineData("12ب34567")]   // Tehran 12, personal series ب
    [InlineData("11ج48514")]   // J-series (ج)
    [InlineData("12د45678")]   // D-series (د)
    [InlineData("78س98765")]   // S-series (س)
    [InlineData("45و11223")]   // V-series (و)
    [InlineData("25ل45678")]   // L-series (ل)
    [InlineData("36م78945")]   // M-series (م)
    [InlineData("87ن23456")]   // N-series (ن)
    [InlineData("23پ56789")]   // police (پ)
    [InlineData("14ت78698")]   // taxi (ت)
    [InlineData("12ث34567")]   // IRGC (ث)
    [InlineData("16ا12345")]   // government (ا), Qom 16
    [InlineData("12ژ34567")]   // disabled/veterans (ژ)
    [InlineData("12ز34567")]   // Ministry of Defence (ز) — Isfahan 67
    [InlineData("12ی34567")]   // Persian letter ی
    [InlineData("12D34567")]   // diplomatic (D)
    [InlineData("78S98765")]   // embassy service (S)
    [InlineData("12ب 345 67")] // with spaces (normalized)
    [InlineData("۱۲ب۳۴۵۶۷")]   // Persian digits (normalized)
    [InlineData("۱۲ي۳۴۵۶۷")]   // Arabic yeh letter -> normalized to ی (valid)
    [InlineData("12ب345ایران67")]     // full form with «ایران», no spaces
    [InlineData("۱۲ ب ۳۴۵ ایران ۶۷")] // full form, Persian digits + spaces
    [InlineData("12ب34510")]   // Tehran 10 — final digit may be 0
    [InlineData("12ب34570")]   // Tehran 70 — final digit may be 0
    public void Validate_ValidPlates_ReturnsSuccess(string plate)
    {
        var result = _sut.Validate(plate.AsSpan());
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().NotBeNull();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    [Fact]
    public void Validate_PersianDigitsNormalized_ReturnsNormalized()
    {
        var result = _sut.Validate("۱۲ب۳۴۵۶۷");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("12ب34567");
    }

    [Fact]
    public void Validate_ArabicYehLetter_NormalizedToPersianYeh()
    {
        var result = _sut.Validate("۱۲ي۳۴۵۶۷");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("12ی34567");
    }

    [Fact]
    public void Validate_WithSpaces_NormalizesToCompact()
    {
        var result = _sut.Validate("12ب 345 67");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("12ب34567");
    }

    [Fact]
    public void Validate_FullFormWithIranWord_NormalizesToCompact()
    {
        var result = _sut.Validate("۱۲ ب ۳۴۵ ایران ۶۷");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("12ب34567");
    }

    [Theory]
    [InlineData("12ب3456")]    // too short (7 chars)
    [InlineData("12ب345678")]  // too long (9 chars)
    [InlineData("1AB34567")]   // letter in digit position
    [InlineData("123ب4567")]   // letter at wrong position (3rd)
    [InlineData("12ب34ع67")]   // letter at wrong position (6th)
    [InlineData("12ب3456X")]   // letter at last position
    [InlineData("AB123456")]   // letters at start
    [InlineData("12@34567")]   // invalid symbol
    [InlineData("12 34567")]   // missing letter
    [InlineData("1_2ب34567")]  // underscore
    public void Validate_InvalidPlates_ReturnsFailure(string plate)
    {
        var result = _sut.Validate(plate.AsSpan());
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().NotBe(ValidationErrorCode.None);
    }

    // Letters that are NOT part of the official issuance series. Latin letters
    // other than D/S and Persian چ خ ر ح ض ظ are never printed on plates.
    [Theory]
    [InlineData("12B34567")]   // Latin B (not an official transliteration)
    [InlineData("12C34567")]   // Latin C
    [InlineData("12W34567")]   // Latin W
    [InlineData("12X34567")]   // Latin X
    [InlineData("12چ34567")]   // چ
    [InlineData("12خ34567")]   // خ
    [InlineData("12ر34567")]   // ر
    [InlineData("12ح34567")]   // ح
    [InlineData("12ض34567")]   // ض
    [InlineData("12ظ34567")]   // ظ
    public void Validate_UnissuedLetters_ReturnsInvalidFormat(string plate)
    {
        var result = _sut.Validate(plate);
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidFormat);
    }

    [Theory]
    [InlineData("12ب34539")]   // 39 — unused/reserved
    [InlineData("12ب34580")]   // 80 — unused/reserved
    [InlineData("12ب34590")]   // 90 — unused/reserved
    public void Validate_UnassignedProvinceCode_ReturnsInvalidProvinceCode(string plate)
    {
        var result = _sut.Validate(plate);
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidProvinceCode);
    }

    [Theory]
    [InlineData("12ب34500")]   // 00 — first province digit is 0
    [InlineData("12ب34509")]   // 09 — first province digit is 0
    [InlineData("10ب34567")]   // 0 in the first digit pair
    [InlineData("12ب04567")]   // 0 in the middle sequence
    [InlineData("12ب34506")]   // 0 in the first province digit
    public void Validate_ZeroInNonFinalDigit_ReturnsInvalidFormat(string plate)
    {
        var result = _sut.Validate(plate);
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidFormat);
    }

    [Theory]
    [InlineData("12ب34511")]   // Tehran 11
    [InlineData("54د45678")]   // Yazd 54
    [InlineData("98س12345")]   // Ilam 98
    [InlineData("99ن88888")]   // Tehran 99
    [InlineData("68گ34567")]   // Alborz 68, temporary (گ)
    [InlineData("32ت34567")]   // Khorasan (shared 32), taxi (ت)
    public void Validate_AssignedProvinceCodes_ReturnsSuccess(string plate)
    {
        var result = _sut.Validate(plate);
        result.Success.Should().BeTrue();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    // === Motorcycle plates (8 digits: 3-digit province code + 5-digit serial) ===

    [Theory]
    [InlineData("11111111")]   // تهران 111
    [InlineData("12345678")]   // تهران 123
    [InlineData("31912345")]   // البرز 319
    [InlineData("37111234")]   // آذربایجان غربی 371
    [InlineData("39234567")]   // آذربایجان شرقی 392
    [InlineData("44212345")]   // اردبیل 442
    [InlineData("46311223")]   // کردستان 463
    [InlineData("48245678")]   // زنجان 482
    [InlineData("51234567")]   // همدان 512
    [InlineData("51411223")]   // کرمانشاه 514
    [InlineData("52712345")]   // قزوین 527
    [InlineData("53711223")]   // مرکزی 537
    [InlineData("53945678")]   // لرستان 539
    [InlineData("54822334")]   // ایلام 548
    [InlineData("55611223")]   // چهارمحال و بختیاری 556
    [InlineData("56134567")]   // خوزستان 561
    [InlineData("57245678")]   // کهگیلویه و بویراحمد 572
    [InlineData("58512345")]   // گیلان 585
    [InlineData("58934567")]   // مازندران 589
    [InlineData("59811223")]   // گلستان 598
    [InlineData("61612345")]   // قم 616
    [InlineData("62745678")]   // اصفهان 627
    [InlineData("64411223")]   // یزد 644
    [InlineData("69934567")]   // فارس 699
    [InlineData("75311223")]   // سمنان 753
    [InlineData("76845678")]   // خراسان رضوی 768
    [InlineData("78212345")]   // خراسان شمالی 782
    [InlineData("79211223")]   // خراسان جنوبی 792
    [InlineData("81734567")]   // کرمان 817
    [InlineData("82445678")]   // سیستان و بلوچستان 824
    [InlineData("83112345")]   // بوشهر 831
    [InlineData("83611223")]   // هرمزگان 836
    [InlineData("87234567")]   // مرکزی/خراسان رضوی (shared 872)
    [InlineData("71112345")]   // فارس/خوزستان (shared 711)
    [InlineData("71245678")]   // فارس/خوزستان (shared 712)
    public void Validate_ValidMotorcycles_ReturnsSuccess(string plate)
    {
        var result = _sut.Validate(plate);
        result.Success.Should().BeTrue();
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    [Fact]
    public void Validate_MotorcyclePersianDigits_NormalizedToLatin()
    {
        var result = _sut.Validate("۱۲۳۴۵۶۷۸");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("12345678");
    }

    [Fact]
    public void Validate_MotorcycleWithSpaces_NormalizesToCompact()
    {
        var result = _sut.Validate("123 45678");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("12345678");
    }

    [Theory]
    [InlineData("1234567")]     // too short (7 digits)
    [InlineData("123456789")]   // too long (9 digits)
    [InlineData("12345670")]    // motorcycle plates never contain 0
    [InlineData("12345067")]    // 0 in the serial
    [InlineData("12045678")]    // 0 in the province code
    [InlineData("11101111")]    // 0 in the serial
    public void Validate_InvalidMotorcycles_ReturnsFailure(string plate)
    {
        var result = _sut.Validate(plate);
        result.Success.Should().BeFalse();
    }

    [Theory]
    [InlineData("99912345")]    // province 999 not assigned
    [InlineData("19912345")]    // province 199 not assigned
    [InlineData("45612345")]    // province 456 not assigned
    public void Validate_UnassignedMotorcycleProvince_ReturnsInvalidProvinceCode(string plate)
    {
        var result = _sut.Validate(plate);
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidProvinceCode);
    }

    [Fact]
    public void Validate_EmptySpan_ReturnsFailure()
    {
        var result = _sut.Validate(ReadOnlySpan<char>.Empty);
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }

    [Fact]
    public void Validate_NullString_ReturnsFailure()
    {
        var result = _sut.Validate((string)null!);
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be(ValidationErrorCode.ValueEmpty);
    }

    [Fact]
    public void Validate_InvalidLength_ReturnsInvalidLength()
    {
        var result = _sut.Validate("12ب3456");
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidLength);
    }

    [Fact]
    public void Validate_InvalidLetter_ReturnsInvalidFormat()
    {
        var result = _sut.Validate("12@34567");
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidFormat);
    }

    [Fact]
    public void Validate_InvalidCharacterInDigits_ReturnsInvalidCharacters()
    {
        var result = _sut.Validate("12ب34A67");
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidCharacters);
    }

    [Theory]
    [InlineData("12ب34567")]
    [InlineData("12ی34567")]
    public void Validate_StringOverload_ReturnsSuccess(string plate)
    {
        var result = _sut.Validate(plate);
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_ConcurrentAccess_NoRaceCondition()
    {
        var plates = new[] { "12ب34567", "11ج48514", "12د45678", "78س98765" };
        var bag = new System.Collections.Concurrent.ConcurrentBag<ValidationResult>();

        Parallel.For(0, 100, i =>
        {
            var result = _sut.Validate(plates[i % plates.Length]);
            bag.Add(result);
        });

        bag.Should().AllSatisfy(r => r.Success.Should().BeTrue());
    }
}

using FluentAssertions;
using IranValidator.Core.Constants;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class TelephoneValidatorTests
{
    private readonly TelephoneValidator _sut = TelephoneValidator.Instance;

    [Theory]
    [InlineData("02122345678")]   // Tehran
    [InlineData("04132645678")]   // Tabriz (East Azerbaijan)
    [InlineData("03132445678")]   // Isfahan
    [InlineData("05138445678")]   // Mashhad (Razavi Khorasan)
    [InlineData("07137245678")]   // Shiraz (Fars)
    [InlineData("08138245678")]   // Hamadan
    [InlineData("06132245678")]   // Ahvaz (Khuzestan)
    [InlineData("02188745678")]   // Tehran, local number starting with 8
    [InlineData("01133245678")]   // Sari (Mazandaran) — 01x codes are valid since the unification plan
    [InlineData("01334245678")]   // Rasht (Gilan)
    [InlineData("01733245678")]   // Gorgan (Golestan)
    [InlineData("02632245678")]   // Karaj (Alborz)
    [InlineData("04434245678")]   // Urmia (West Azerbaijan)
    [InlineData("08338245678")]   // Kermanshah
    [InlineData("08734245678")]   // Sanandaj (Kurdistan)
    [InlineData("07734245678")]   // Bushehr
    public void Validate_ValidNumbers_ReturnsSuccess(string telephone)
    {
        var result = _sut.Validate(telephone);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567")]       // Too short
    [InlineData("021223456789")]  // Too long (12 digits)
    [InlineData("0212234567")]    // Too short (10 digits)
    [InlineData("02100000000")]   // Local number starting with 0 (trunk prefix)
    [InlineData("02112345678")]   // Local number starting with 1 (1xx = service numbers)
    [InlineData("09121234567")]   // Mobile number — 91 is not a landline area code
    [InlineData("09001234567")]   // Unassigned area code (90)
    [InlineData("01212345678")]   // Unassigned area code (12)
    [InlineData("01912345678")]   // Unassigned area code (19)
    [InlineData("02912345678")]   // Unassigned area code (29)
    [InlineData("09912345678")]   // Unassigned area code (99)
    [InlineData("021abcd5678")]   // Contains letters
    public void Validate_InvalidNumbers_ReturnsFailure(string telephone)
    {
        var result = _sut.Validate(telephone);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_EveryProvinceAreaCode_ReturnsSuccess()
    {
        foreach (ushort code in ProvinceAreaCodes.Valid)
        {
            var result = _sut.Validate($"0{code:00}22345678");
            result.Success.Should().BeTrue($"area code {code:00} is assigned to a province");
        }
    }

    [Fact]
    public void ProvinceAreaCodes_IsSortedUniqueAndWithinRange()
    {
        var codes = ProvinceAreaCodes.Valid;
        codes.Should().BeInAscendingOrder();
        codes.Should().OnlyHaveUniqueItems();
        codes.Should().OnlyContain(p => p >= 11 && p <= 87);
        codes.Should().HaveCount(31);
    }

    [Fact]
    public void Validate_Null_ReturnsFailure()
    {
        var result = _sut.Validate(null!);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptyString_ReturnsFailure()
    {
        var result = _sut.Validate(string.Empty);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_RtlMarker_ReturnsSuccess()
    {
        var result = _sut.Validate("\u200F02122345678");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDashes_NormalizesCorrectly()
    {
        var result = _sut.Validate("021-2234-5678");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_PersianDigits_ReturnsSuccess()
    {
        var result = _sut.Validate("۰۲۱۲۲۳۴۵۶۷۸");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithSpaces_NormalizesCorrectly()
    {
        var result = _sut.Validate("021 2234 5678");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Instance_IsSingleton()
    {
        TelephoneValidator.Instance.Should().BeSameAs(TelephoneValidator.Instance);
    }
}

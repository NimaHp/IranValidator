using FluentAssertions;
using IranValidator.Core.Validators;
using Xunit;

namespace IranValidator.Tests.Core.Validators;

public class CompanyIdValidatorTests
{
    private readonly CompanyIdValidator _sut = CompanyIdValidator.Instance;

    [Theory]
    [InlineData("10380284795")]
    [InlineData("10380058722")]
    [InlineData("10260353695")]
    [InlineData("14005124960")]
    [InlineData("10790116961")]
    public void Validate_ValidCompanyIds_ReturnsSuccess(string companyId)
    {
        var result = _sut.Validate(companyId);
        result.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567890")]          // 10 digits
    [InlineData("123456789012")]         // 12 digits
    [InlineData("00000000000")]          // All zeros
    [InlineData("11111111111")]          // All ones
    [InlineData("abcdefghijk")]          // Letters
    [InlineData("10380284791")]          // Wrong checksum
    public void Validate_InvalidCompanyIds_ReturnsFailure(string companyId)
    {
        var result = _sut.Validate(companyId);
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_AllSameDigits_AllVariants()
    {
        for (int d = 0; d <= 9; d++)
        {
            string code = new string((char)('0' + d), 11);
            _sut.Validate(code).Success.Should().BeFalse($"all-{d}s should fail");
        }
    }

    [Fact]
    public void Validate_Null_ReturnsFailure()
    {
        _sut.Validate(null!).Success.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithPersianDigits_NormalizesCorrectly()
    {
        var result = _sut.Validate("۱۰۳۸۰۲۸۴۷۹۵");
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Instance_IsSingleton()
    {
        CompanyIdValidator.Instance.Should().BeSameAs(CompanyIdValidator.Instance);
    }
}

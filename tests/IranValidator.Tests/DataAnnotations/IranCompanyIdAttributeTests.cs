using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace IranValidator.Tests.DataAnnotations;

public class IranCompanyIdAttributeTests
{
    [Theory]
    [InlineData("10380284795")]
    [InlineData("14005124960")]
    [InlineData("10260353695")]
    [InlineData("10790116961")]
    public void IsValid_ValidCompanyId_ReturnsSuccess(string companyId)
    {
        var attr = new IranValidator.DataAnnotations.IranCompanyIdAttribute();
        var result = attr.GetValidationResult(companyId, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("10380284791")]
    [InlineData("11111111111")]
    [InlineData("abcdefghijk")]
    [InlineData("")]
    public void IsValid_InvalidCompanyId_ReturnsError(string companyId)
    {
        var attr = new IranValidator.DataAnnotations.IranCompanyIdAttribute();
        var result = attr.GetValidationResult(companyId, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        var attr = new IranValidator.DataAnnotations.IranCompanyIdAttribute();
        var result = attr.GetValidationResult(null, new ValidationContext(new { }));
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_NonStringValue_ReturnsError()
    {
        var attr = new IranValidator.DataAnnotations.IranCompanyIdAttribute();
        var result = attr.GetValidationResult(12345, new ValidationContext(new { }));
        result.Should().NotBe(ValidationResult.Success);
    }

}

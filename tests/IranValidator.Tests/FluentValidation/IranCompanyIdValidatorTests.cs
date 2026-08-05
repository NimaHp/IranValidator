using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

public class IranCompanyIdValidatorTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    private readonly InlineValidator<TestModel> _validator = new();

    [Theory]
    [InlineData("10380284795")]
    [InlineData("14005124960")]
    [InlineData("10260353695")]
    public void IranCompanyId_ValidValue_Passes(string companyId)
    {
        _validator.RuleFor(x => x.Value).IranCompanyId();
        var result = _validator.Validate(new TestModel { Value = companyId });
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("10380284791")]
    public void IranCompanyId_InvalidValue_Fails(string companyId)
    {
        _validator.RuleFor(x => x.Value).IranCompanyId();
        var result = _validator.Validate(new TestModel { Value = companyId });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IranCompanyId_Null_Passes()
    {
        _validator.RuleFor(x => x.Value).IranCompanyId();
        var result = _validator.Validate(new TestModel { Value = null });
        result.IsValid.Should().BeTrue();
    }
}

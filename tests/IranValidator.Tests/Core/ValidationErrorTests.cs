using FluentAssertions;
using IranValidator.Core.Results;
using Xunit;

namespace IranValidator.Tests.Core;

public class ValidationErrorTests
{
    [Fact]
    public void Constructor_WithCode_SetsProperties()
    {
        var error = new ValidationError(ValidationErrorCode.InvalidLength);
        error.Code.Should().Be(ValidationErrorCode.InvalidLength);
        error.PropertyName.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithCodeAndProperty_SetsProperties()
    {
        var error = new ValidationError(ValidationErrorCode.InvalidFormat, "Mobile");
        error.Code.Should().Be(ValidationErrorCode.InvalidFormat);
        error.PropertyName.Should().Be("Mobile");
    }

    [Fact]
    public void Constructor_WithNoneCode_WorksCorrectly()
    {
        var error = new ValidationError(ValidationErrorCode.None);
        error.Code.Should().Be(ValidationErrorCode.None);
    }

    [Fact]
    public void Constructor_WithNullPropertyName_SetsNull()
    {
        var error = new ValidationError(ValidationErrorCode.ValueEmpty, null);
        error.PropertyName.Should().BeNull();
    }
}

public class ValidationResultTests
{
    [Fact]
    public void Ok_CreatesSuccessResult()
    {
        var result = ValidationResult.Ok("normalized");
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().Be("normalized");
        result.ErrorCode.Should().Be(ValidationErrorCode.None);
    }

    [Fact]
    public void Ok_WithNullValue_CreatesSuccessResult()
    {
        var result = ValidationResult.Ok(null);
        result.Success.Should().BeTrue();
        result.NormalizedValue.Should().BeNull();
    }

    [Fact]
    public void Error_CreatesFailureResult()
    {
        var result = ValidationResult.Error(ValidationErrorCode.InvalidChecksum);
        result.Success.Should().BeFalse();
        result.NormalizedValue.Should().BeNull();
        result.ErrorCode.Should().Be(ValidationErrorCode.InvalidChecksum);
    }

    [Fact]
    public void Error_WithDifferentCodes_SetsCorrectly()
    {
        foreach (var code in new[] {
            ValidationErrorCode.ValueEmpty,
            ValidationErrorCode.InvalidLength,
            ValidationErrorCode.InvalidChecksum,
            ValidationErrorCode.InvalidFormat,
            ValidationErrorCode.InvalidCharacters
        })
        {
            var result = ValidationResult.Error(code);
            result.Success.Should().BeFalse();
            result.ErrorCode.Should().Be(code);
        }
    }

    [Fact]
    public void Ok_NormalizedValue_IsPreserved()
    {
        var result = ValidationResult.Ok("0010350829");
        result.NormalizedValue.Should().Be("0010350829");
    }
}

using System.Globalization;
using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Localization;
using Xunit;

namespace IranValidator.Tests.Localization;

public class PersianMessageResolverTests
{
    private readonly PersianMessageResolver _resolver = new();

    [Theory]
    [InlineData(ValidationErrorCode.ValueEmpty, "نمی‌تواند خالی باشد")]
    [InlineData(ValidationErrorCode.InvalidLength, "طول")]
    [InlineData(ValidationErrorCode.InvalidChecksum, "مجموع ارقام")]
    [InlineData(ValidationErrorCode.InvalidFormat, "فرمت")]
    [InlineData(ValidationErrorCode.InvalidCharacters, "کاراکترهای نامعتبر")]
    [InlineData(ValidationErrorCode.InvalidProvinceCode, "کد استان")]
    [InlineData(ValidationErrorCode.InvalidBankCode, "کد بانک")]
    [InlineData(ValidationErrorCode.InvalidAreaCode, "پیش‌شماره")]
    [InlineData(ValidationErrorCode.UnsupportedIssuer, "بانک ایرانی")]
    public void GetMessage_WithPropertyName_ContainsCorrectPersianKeywords(ValidationErrorCode code, string expectedKeyword)
    {
        var msg = _resolver.GetMessage(code, "موبایل", null);
        msg.Should().Contain(expectedKeyword);
    }

    [Fact]
    public void GetMessage_WithoutPropertyName_UsesPersianDefault()
    {
        var msg = _resolver.GetMessage(ValidationErrorCode.InvalidLength, null, null);
        msg.Should().Be("طول مقدار نامعتبر است.");
    }

    [Fact]
    public void GetMessage_PropertyNameIsIncluded()
    {
        var msg = _resolver.GetMessage(ValidationErrorCode.InvalidFormat, "کد ملی", null);
        msg.Should().Contain("کد ملی");
    }

    [Fact]
    public void GetMessage_UnhandledErrorCode_ReturnsGenericPersianMessage()
    {
        // ValidationErrorCode.None is not matched by the explicit switch arms
        var msg = _resolver.GetMessage(ValidationErrorCode.None, "کد ملی", null);
        msg.Should().Be("کد ملی معتبر نیست.");
    }

}

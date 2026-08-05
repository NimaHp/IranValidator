using System.Globalization;
using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.Localization;
using Xunit;

namespace IranValidator.Tests.Localization;

public class CurrentCultureResolverTests
{
    [Fact]
    public void Ctor_NullOptions_Throws()
    {
        var act = () => new CurrentCultureResolver(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public void GetMessage_ExplicitCulture_WinsOverCurrentUICulture()
    {
        var options = new ValidationMessageOptions();
        options.AddResolver(CultureInfo.GetCultureInfo("fa"), new PersianMessageResolver());
        var resolver = new CurrentCultureResolver(options);

        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var message = resolver.GetMessage(
                ValidationErrorCode.InvalidFormat, "Code", CultureInfo.GetCultureInfo("fa-IR"));
            message.Should().Be("فرمت Code نامعتبر است.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }
}

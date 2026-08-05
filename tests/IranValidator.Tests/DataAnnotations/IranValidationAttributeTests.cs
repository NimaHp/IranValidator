using System.ComponentModel.DataAnnotations;
using System.Globalization;
using FluentAssertions;
using IranValidator.Core.Results;
using IranValidator.DataAnnotations;
using IranValidator.Localization;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using Xunit;

namespace IranValidator.Tests.DataAnnotations;

/// <summary>
/// Covers the message resolution strategy in <see cref="IranValidationAttribute"/>:
/// a resolver provided by the validation context's service provider wins
/// (ASP.NET Core DI path), otherwise the static registry is used.
/// </summary>
public class IranValidationAttributeTests
{
    [Fact]
    public void IsValid_ServiceProviderResolver_TakesPrecedence()
    {
        var attr = new IranMobileAttribute();
        var context = new ValidationContext(new object(), new StubServiceProvider(), items: null)
        {
            DisplayName = "Mobile"
        };

        var result = attr.GetValidationResult("091212345678", context);

        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Be("Mobile [InvalidLength]");
    }

    [Fact]
    public void IsValid_ServiceProviderWithoutResolver_FallsBackToStatic()
    {
        var attr = new IranMobileAttribute();
        var context = new ValidationContext(new object(), new EmptyServiceProvider(), items: null)
        {
            DisplayName = "Mobile"
        };

        var before = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var result = attr.GetValidationResult("091212345678", context);

            result.Should().NotBe(ValidationResult.Success);
            result!.ErrorMessage.Should().Be("Mobile has an invalid length.");
        }
        finally
        {
            CultureInfo.CurrentUICulture = before;
        }
    }

    private sealed class StubServiceProvider : IServiceProvider
    {
        private readonly IValidationMessageResolver _resolver = new StubResolver();

        public object? GetService(Type serviceType)
            => serviceType == typeof(IValidationMessageResolver) ? _resolver : null;
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }

    private sealed class StubResolver : IValidationMessageResolver
    {
        public string GetMessage(ValidationErrorCode errorCode, string? propertyName, CultureInfo? culture)
            => $"{propertyName ?? "Value"} [{errorCode}]";
    }
}

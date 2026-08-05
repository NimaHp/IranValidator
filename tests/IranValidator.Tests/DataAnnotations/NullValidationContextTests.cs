using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using IranValidator.DataAnnotations;
using Xunit;

namespace IranValidator.Tests.DataAnnotations;

/// <summary>
/// Covers the defensive null-ValidationContext path in every DataAnnotations
/// attribute (<c>validationContext?.DisplayName ?? "The field"</c>). The path is
/// reachable through the protected two-argument <see cref="ValidationAttribute.IsValid(object,ValidationContext)"/>
/// override when the base class passes a null context (e.g. via the public
/// single-argument <see cref="ValidationAttribute.IsValid(object)"/> overload),
/// so we invoke that override directly with a null context.
/// </summary>
public class NullValidationContextTests
{
    public static TheoryData<ValidationAttribute> Attributes => new()
    {
        new NationalCodeAttribute(),
        new IranCardNumberAttribute(),
        new IranCompanyIdAttribute(),
        new IranEconomicCodeAttribute(),
        new IranMobileAttribute(),
        new IranPassportAttribute(),
        new IranPostalCodeAttribute(),
        new IranTelephoneAttribute(),
        new IranVehiclePlateAttribute(),
        new IranIbanAttribute(),
    };

    private static ValidationResult? InvokeProtectedIsValid(ValidationAttribute attribute, object? value)
    {
        var method = typeof(ValidationAttribute).GetMethod(
            nameof(ValidationAttribute.IsValid),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(object), typeof(ValidationContext)],
            modifiers: null);

        return (ValidationResult?)method!.Invoke(attribute, [value, null]);
    }

    [Theory]
    [MemberData(nameof(Attributes))]
    public void IsValid_NullValidationContext_UsesDefaultDisplayName(ValidationAttribute attribute)
    {
        var result = InvokeProtectedIsValid(attribute, "invalid");

        result.Should().NotBeNull();
        result!.ErrorMessage.Should().Contain("The field");
    }
}

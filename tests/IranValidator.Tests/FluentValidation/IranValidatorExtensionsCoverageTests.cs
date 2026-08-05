using FluentAssertions;
using FluentValidation;
using IranValidator.FluentValidation;
using Xunit;

namespace IranValidator.Tests.FluentValidation;

/// <summary>
/// Matrix coverage for all 10 FluentValidation rule extensions: each rule is
/// exercised with null, empty, valid and invalid values so the shared
/// null/empty fast-path branches and the localized message builder are covered
/// for every rule, not just the two with dedicated test classes.
/// </summary>
public class IranValidatorExtensionsCoverageTests
{
    private sealed class TestModel { public string? Value { get; set; } }

    [Fact]
    public void AllRules_NullEmptyValidInvalid_BehaveConsistently()
    {
        var cases = new (string Name, Action<IRuleBuilder<TestModel, string?>> Rule, string Valid, string Invalid)[]
        {
            ("NationalCode", r => r.IranNationalCode(), "0010350829", "0000000000"),
            ("CardNumber", r => r.IranCardNumber(), "6037991234567893", "6037991234567892"),
            ("CompanyId", r => r.IranCompanyId(), "10380284795", "10380284796"),
            ("EconomicCode", r => r.IranEconomicCode(), "123456789019", "123456789018"),
            ("Mobile", r => r.IranMobile(), "09121234567", "091212345678"),
            ("Passport", r => r.IranPassport(), "P12345678", "P1234567"),
            ("PostalCode", r => r.IranPostalCode(), "1234567890", "12345"),
            ("Telephone", r => r.IranTelephone(), "02122345678", "021123456"),
            ("VehiclePlate", r => r.IranVehiclePlate(), "12ب34567", "12ب3456"),
            ("Iban", r => r.IranIban(), "IR820540102680020817909002", "IR489991234567890123456789"),
        };

        foreach (var (name, rule, valid, invalid) in cases)
        {
            var validator = new InlineValidator<TestModel>();
            rule(validator.RuleFor(x => x.Value));

            validator.Validate(new TestModel { Value = null }).IsValid.Should().BeTrue(name);
            validator.Validate(new TestModel { Value = "" }).IsValid.Should().BeTrue(name);
            validator.Validate(new TestModel { Value = valid }).IsValid.Should().BeTrue(name);
            validator.Validate(new TestModel { Value = invalid }).IsValid.Should().BeFalse(name);
        }
    }
}

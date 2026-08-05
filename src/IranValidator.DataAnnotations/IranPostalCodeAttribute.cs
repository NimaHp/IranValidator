using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian postal code.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranPostalCodeAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IranPostalCodeAttribute"/> class.
    /// </summary>
    public IranPostalCodeAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => PostalCodeValidator.Instance;
}

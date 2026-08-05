using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian mobile number.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranMobileAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IranMobileAttribute"/> class.
    /// </summary>
    public IranMobileAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => MobileValidator.Instance;
}

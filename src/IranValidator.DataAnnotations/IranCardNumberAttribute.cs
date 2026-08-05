using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian bank card number.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranCardNumberAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IranCardNumberAttribute"/> class.
    /// </summary>
    public IranCardNumberAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => CardNumberValidator.Instance;
}

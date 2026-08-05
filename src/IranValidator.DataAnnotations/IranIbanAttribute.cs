using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian IBAN (شماره شبا).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranIbanAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IranIbanAttribute"/> class.
    /// </summary>
    public IranIbanAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => IbanValidator.Instance;
}

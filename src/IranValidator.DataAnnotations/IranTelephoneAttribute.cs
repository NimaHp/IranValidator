using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian landline telephone number.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranTelephoneAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IranTelephoneAttribute"/> class.
    /// </summary>
    public IranTelephoneAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => TelephoneValidator.Instance;
}

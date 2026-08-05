using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian Passport Number (شماره گذرنامه).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranPassportAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IranPassportAttribute"/> class.
    /// </summary>
    public IranPassportAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => PassportValidator.Instance;
}

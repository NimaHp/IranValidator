using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian national code (کد ملی).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class NationalCodeAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NationalCodeAttribute"/> class.
    /// </summary>
    public NationalCodeAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => NationalCodeValidator.Instance;
}

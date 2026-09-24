using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian Passport Number (شماره گذرنامه).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranPassportAttribute : IranValidationAttribute
{
    private readonly IStringValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="IranPassportAttribute"/> class.
    /// </summary>
    public IranPassportAttribute()
        : this(false)
    {
    }

    public IranPassportAttribute(bool allowLegacy8Digit)
    {
        _validator = allowLegacy8Digit
            ? PassportValidator.CreateArchiveValidator()
            : PassportValidator.Instance;
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => _validator;
}
